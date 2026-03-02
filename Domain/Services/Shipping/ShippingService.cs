using Domain.Constants;
using Domain.Data.Models;
using Domain.Enumerators;
using Domain.Exceptions;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Domain.Services;

public class ShippingService : IShippingService
{
    private readonly HttpClient _httpClient;

    private readonly string _apiToken;
    private readonly string _fromZipCode;

    public ShippingService(IHttpClientFactory httpClientFactory) 
    {
        _apiToken = Constant.Settings.ShippingServiceSettings.ServiceAPIKey;
        _fromZipCode = Constant.Settings.ShippingServiceSettings.ShippingPostalCode;

        _httpClient = httpClientFactory.CreateClient(Constant.Settings.ShippingServiceSettings.ShippingServiceName);

        _httpClient.BaseAddress = new Uri(Constant.Settings.ShippingServiceSettings.ServiceShippingEndpoint);

        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Tallow-API");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiToken);
    }

    public async Task<List<ShippingQuoteResponse>> CalculateShippingAsync(ShippingQuoteRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var superFreteRequest = new SuperFreteRequest
            {
                From = new FromAddress
                {
                    PostalCode = _fromZipCode.Replace("-", "").Replace(".", "").Trim()
                },

                To = new ToAddress
                {
                    PostalCode = request.ToZipCode.Replace("-", "").Replace(".", "").Trim()
                },

                Services = "1,2,17,3,31",

                Options = new ShippingOptions
                {
                    OwnHand = false,
                    Receipt = false,
                    InsuranceValue = request.DeclaredValue,
                    UseInsuranceValue = request.DeclaredValue > 0
                },

                Products = new List<ProductInfo>
                {
                    new ProductInfo
                    {
                        Quantity = request.Quantity > 0 ? request.Quantity : 1,
                        Weight = request.Weight,
                        Height = request.Height,
                        Width = request.Width,
                        Length = request.Length
                    }
                }
            };

            var jsonContent = JsonSerializer.Serialize(superFreteRequest, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/v0/calculator", content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new BusinessException(BusinessErrorMessage.SOMETHING_WENT_WRONG);
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var superFreteResponses = JsonSerializer.Deserialize<List<SuperFreteResponse>>(responseContent, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return superFreteResponses?.Select(sf => new ShippingQuoteResponse
            {
                CarrierName = sf.Company?.Name ?? string.Empty,
                CarrierCode = sf.Company?.Id.ToString() ?? string.Empty,
                ServiceName = sf.Name,
                ServiceCode = sf.Id.ToString(),
                DeliveryPrice = ParseDecimalFromString(sf.Price),
                DeliveryTime = sf.DeliveryTime,
                Error = sf.Error
            }).Where(x => string.IsNullOrEmpty(x.Error)).ToList() ?? new List<ShippingQuoteResponse>();
        }
        catch (HttpRequestException)
        {
            throw new BusinessException(BusinessErrorMessage.SOMETHING_WENT_WRONG);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ShippingQuoteResponse> CalculateCheapestShippingAsync(ShippingQuoteRequest request, CancellationToken cancellationToken = default)
    {
        var quotes = await CalculateShippingAsync(request, cancellationToken);

        return quotes.OrderBy(x => x.DeliveryPrice).FirstOrDefault()
            ?? throw new BusinessException(BusinessErrorMessage.SOMETHING_WENT_WRONG);
    }



    #region .: HELPER METHODS :.

    private static decimal ParseDecimalFromString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return 0m;

        value = value.Trim().Replace(",", ".");

        if (decimal.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var result))
            return result;

        return 0m;
    }

    #endregion .: HELPER METHODS :.
}
