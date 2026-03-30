using Domain.Constants;
using Domain.Data.Entities;
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

    public async Task<ShippingQuoteResponse> CalculateFastestShippingAsync(ShippingQuoteRequest request, CancellationToken cancellationToken = default)
    {
        var quotes = await CalculateShippingAsync(request, cancellationToken);

        return quotes.OrderBy(x => x.DeliveryTime).FirstOrDefault()
            ?? throw new BusinessException(BusinessErrorMessage.SOMETHING_WENT_WRONG);
    }

    public async Task<List<ShippingQuoteResponse>> CalculateCartShippingAsync(CartShippingQuoteRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var totalInsuranceValue = request.Products.Sum(p => p.DeclaredValue);

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
                    InsuranceValue = totalInsuranceValue,
                    UseInsuranceValue = totalInsuranceValue > 0
                },

                Products = request.Products.Select(p => new ProductInfo
                {
                    Quantity = p.Quantity > 0 ? p.Quantity : 1,
                    Weight = p.Weight,
                    Height = p.Height,
                    Width = p.Width,
                    Length = p.Length
                }).ToList()
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

    public async Task<ShippingQuoteResponse> CalculateFastestCartShippingAsync(CartShippingQuoteRequest request, CancellationToken cancellationToken = default)
    {
        var quotes = await CalculateCartShippingAsync(request, cancellationToken);

        return quotes.OrderBy(x => x.DeliveryTime).FirstOrDefault()
            ?? throw new BusinessException(BusinessErrorMessage.SOMETHING_WENT_WRONG);
    }



    public async Task<ShipmentResult> CreateShipmentAsync(
        Order order,
        int serviceCode,
        float packageWeight,
        float packageHeight,
        float packageWidth,
        float packageLength,
        CancellationToken cancellationToken = default)
    {
        var settings = Constant.Settings.ShippingServiceSettings;
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        // Step 1: Add order to SuperFrete cart
        var cartRequest = new SuperFreteOrderRequest
        {
            Service = serviceCode,
            From = new SuperFreteAddress
            {
                Name = settings.SenderName,
                Document = settings.SenderDocument,
                Email = settings.SenderEmail,
                Phone = settings.SenderPhone,
                Address = settings.SenderAddress,
                Number = settings.SenderAddressNumber,
                District = settings.SenderDistrict,
                City = settings.SenderCity,
                StateAbbr = settings.SenderStateAbbr,
                PostalCode = settings.ShippingPostalCode.Replace("-", "").Trim()
            },
            To = new SuperFreteAddress
            {
                Name = order.BuyerName,
                Document = order.BuyerDocument?.Replace(".", "").Replace("-", "") ?? string.Empty,
                Email = order.BuyerEmail,
                Phone = order.BuyerCellphone?.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "") ?? string.Empty,
                Address = order.ShippingAddress,
                Complement = order.ShippingComplement,
                Number = order.ShippingNumber,
                District = order.ShippingNeighborhood,
                City = order.ShippingCity,
                StateAbbr = order.ShippingState,
                PostalCode = order.ShippingZipcode?.Replace("-", "").Trim() ?? string.Empty
            },
            Products = new List<SuperFreteProduct>
            {
                new SuperFreteProduct
                {
                    Name = $"Pedido #{order.Id[..8]}",
                    Quantity = order.Items?.Sum(i => i.Quantity) ?? 1,
                    UnitaryValue = order.SubTotalAmount,
                    Weight = packageWeight
                }
            },
            Volumes = new List<SuperFreteVolume>
            {
                new SuperFreteVolume
                {
                    Height = packageHeight,
                    Width = packageWidth,
                    Length = packageLength,
                    Weight = packageWeight
                }
            },
            Options = new SuperFreteOrderOptions
            {
                InsuranceValue = order.TotalAmount,
                Receipt = false,
                OwnHand = false,
                Collect = false,
                Reverse = false,
                NonCommercial = false
            }
        };

        var cartJson = JsonSerializer.Serialize(cartRequest, options);
        var cartContent = new StringContent(cartJson, Encoding.UTF8, "application/json");
        var cartResponse = await _httpClient.PostAsync("api/v0/cart", cartContent, cancellationToken);

        if (!cartResponse.IsSuccessStatusCode)
            throw new BusinessException(BusinessErrorMessage.SOMETHING_WENT_WRONG);

        var cartBody = await cartResponse.Content.ReadAsStringAsync(cancellationToken);
        var cartResult = JsonSerializer.Deserialize<SuperFreteOrderResponse>(cartBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (cartResult?.Id == null || !string.IsNullOrEmpty(cartResult.Error))
            throw new BusinessException(BusinessErrorMessage.SOMETHING_WENT_WRONG);

        // Step 2: Pay for the cart — this generates the shipping label
        var payRequest = new SuperFreteCartPaymentRequest { Orders = new List<string> { cartResult.Id } };
        var payJson = JsonSerializer.Serialize(payRequest, options);
        var payContent = new StringContent(payJson, Encoding.UTF8, "application/json");
        var payResponse = await _httpClient.PostAsync("api/v0/cart/payment", payContent, cancellationToken);

        if (!payResponse.IsSuccessStatusCode)
            throw new BusinessException(BusinessErrorMessage.SOMETHING_WENT_WRONG);

        var payBody = await payResponse.Content.ReadAsStringAsync(cancellationToken);
        var paidOrders = JsonSerializer.Deserialize<List<SuperFreteOrderResponse>>(payBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var paid = paidOrders?.FirstOrDefault();

        return new ShipmentResult
        {
            SuperFreteOrderId = cartResult.Id,
            TrackingCode = paid?.Tracking ?? cartResult.Tracking,
            LabelUrl = paid?.Print ?? cartResult.Print,
            TrackingUrl = paid?.Tracking != null
                ? $"https://superfrete.com/rastreio/{paid.Tracking}"
                : null
        };
    }

    public async Task<SuperFreteTrackingResponse> GetTrackingAsync(string superFreteOrderId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/v0/tracking/{superFreteOrderId}", cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new BusinessException(BusinessErrorMessage.SOMETHING_WENT_WRONG);

        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        return JsonSerializer.Deserialize<SuperFreteTrackingResponse>(body, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new SuperFreteTrackingResponse();
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
