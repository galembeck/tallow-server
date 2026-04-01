using Domain.Constants;
using Domain.Data.Entities;
using Domain.Data.Models;
using Domain.Enumerators;
using Domain.Exceptions;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Domain.Services;

public class ShippingService : IShippingService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ShippingService> _logger;

    private readonly string _apiToken;
    private readonly string _fromZipCode;

    private static readonly JsonSerializerOptions _serializeOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    private static readonly JsonSerializerOptions _deserializeOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ShippingService(IHttpClientFactory httpClientFactory, ILogger<ShippingService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
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

            var jsonContent = JsonSerializer.Serialize(superFreteRequest, _serializeOptions);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/v0/calculator", content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await response.Content.ReadAsStringAsync(cancellationToken);
                throw new BusinessException(BusinessErrorMessage.SOMETHING_WENT_WRONG);
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var superFreteResponses = JsonSerializer.Deserialize<List<SuperFreteResponse>>(responseContent, _deserializeOptions);

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

            var jsonContent = JsonSerializer.Serialize(superFreteRequest, _serializeOptions);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/v0/calculator", content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await response.Content.ReadAsStringAsync(cancellationToken);
                throw new BusinessException(BusinessErrorMessage.SOMETHING_WENT_WRONG);
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var superFreteResponses = JsonSerializer.Deserialize<List<SuperFreteResponse>>(responseContent, _deserializeOptions);

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

    public async Task<SuperFreteCartResponse> AddOrderToCartAsync(Order order, int serviceId, ShipmentCartOptions? options = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var settings = Constant.Settings.ShippingServiceSettings;

            // Aggregate package dimensions from all order items
            var totalWeight = order.Items.Sum(i => i.Product.Weight * i.Quantity);
            var maxHeight   = order.Items.Max(i => i.Product.Height);
            var maxWidth    = order.Items.Max(i => i.Product.Width);
            var maxLength   = order.Items.Max(i => i.Product.Length);

            var insuranceValue = options?.InsuranceValue ?? order.TotalAmount;

            var cartRequest = new SuperFreteCartRequest
            {
                Service = serviceId,

                From = new SuperFreteCartSenderAddress
                {
                    Name       = settings.ShippingFromName,
                    Address    = settings.ShippingFromAddress,
                    Number     = settings.ShippingFromNumber,
                    Complement = settings.ShippingFromComplement,
                    District   = string.IsNullOrWhiteSpace(settings.ShippingFromDistrict) ? "NA" : settings.ShippingFromDistrict,
                    City       = settings.ShippingFromCity,
                    StateAbbr  = settings.ShippingFromState.ToUpperInvariant(),
                    PostalCode = settings.ShippingPostalCode.Replace("-", "").Replace(".", "").Trim(),
                    Document   = settings.ShippingFromDocument
                },

                To = new SuperFreteCartRecipientAddress
                {
                    Name       = order.BuyerName,
                    Address    = order.ShippingAddress,
                    Number     = string.IsNullOrWhiteSpace(order.ShippingNumber) ? "" : order.ShippingNumber,
                    Complement = order.ShippingComplement,
                    District   = string.IsNullOrWhiteSpace(order.ShippingNeighborhood) ? "NA" : order.ShippingNeighborhood,
                    City       = order.ShippingCity,
                    StateAbbr  = order.ShippingState.ToUpperInvariant(),
                    PostalCode = order.ShippingZipcode.Replace("-", "").Replace(".", "").Trim(),
                    Document   = order.BuyerDocument,
                    Email      = order.BuyerEmail
                },

                Volumes = new SuperFreteCartVolumes
                {
                    Weight = (float)totalWeight,
                    Height = maxHeight,
                    Width  = maxWidth,
                    Length = maxLength
                },

                Products = order.Items.Select(i => new SuperFreteCartProduct
                {
                    Name         = i.ProductName,
                    Quantity     = i.Quantity,
                    UnitaryValue = i.UnitPrice
                }).ToList(),

                Options = new SuperFreteCartShippingOptions
                {
                    InsuranceValue = insuranceValue,
                    Receipt        = options?.Receipt ?? false,
                    OwnHand        = options?.OwnHand ?? false,
                    NonCommercial  = options?.NonCommercial ?? false,
                    Invoice        = options?.InvoiceNumber is not null
                        ? new SuperFreteCartInvoice { Number = options.InvoiceNumber, Key = options.InvoiceKey }
                        : null
                }
            };

            var jsonContent = JsonSerializer.Serialize(cartRequest, _serializeOptions);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/v0/cart", content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new BusinessException(BusinessErrorMessage.SOMETHING_WENT_WRONG);
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<SuperFreteCartResponse>(responseContent, _deserializeOptions)
                ?? throw new BusinessException(BusinessErrorMessage.SOMETHING_WENT_WRONG);
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

    public async Task<SuperFreteCheckoutResponse> CheckoutOrderAsync(string superFreteOrderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var body = new { orders = new[] { superFreteOrderId } };
            var jsonContent = JsonSerializer.Serialize(body, _serializeOptions);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/v0/checkout", content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await response.Content.ReadAsStringAsync(cancellationToken);
                throw new BusinessException(BusinessErrorMessage.SOMETHING_WENT_WRONG);
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogInformation("[SuperFrete] Checkout raw response: {Json}", responseContent);

            return JsonSerializer.Deserialize<SuperFreteCheckoutResponse>(responseContent, _deserializeOptions)
                ?? throw new BusinessException(BusinessErrorMessage.SOMETHING_WENT_WRONG);
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

    public async Task<SuperFretePrintResponse> PrintLabelAsync(string superFreteOrderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var body = new { orders = new[] { superFreteOrderId } };
            var jsonContent = JsonSerializer.Serialize(body, _serializeOptions);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/v0/tag/print", content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await response.Content.ReadAsStringAsync(cancellationToken);
                throw new BusinessException(BusinessErrorMessage.SOMETHING_WENT_WRONG);
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<SuperFretePrintResponse>(responseContent, _deserializeOptions)
                ?? throw new BusinessException(BusinessErrorMessage.SOMETHING_WENT_WRONG);
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

    public async Task<SuperFreteOrderInfoResponse> GetOrderInfoAsync(string superFreteOrderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/v0/order/info/{superFreteOrderId}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await response.Content.ReadAsStringAsync(cancellationToken);
                throw new BusinessException(BusinessErrorMessage.SOMETHING_WENT_WRONG);
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogInformation("[SuperFrete] OrderInfo raw response: {Json}", responseContent);

            return JsonSerializer.Deserialize<SuperFreteOrderInfoResponse>(responseContent, _deserializeOptions)
                ?? throw new BusinessException(BusinessErrorMessage.SOMETHING_WENT_WRONG);
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

    public async Task<bool> CancelOrderAsync(string superFreteOrderId, string description = "Cancelado pelo usuário", CancellationToken cancellationToken = default)
    {
        try
        {
            var body = new { order = new { id = superFreteOrderId, description } };
            var jsonContent = JsonSerializer.Serialize(body, _serializeOptions);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/v0/order/cancel", content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await response.Content.ReadAsStringAsync(cancellationToken);
                throw new BusinessException(BusinessErrorMessage.SOMETHING_WENT_WRONG);
            }

            // Response is { "{orderId}": { "canceled": true } }
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(responseContent);

            foreach (var prop in doc.RootElement.EnumerateObject())
            {
                if (prop.Value.TryGetProperty("canceled", out var canceledProp))
                    return canceledProp.GetBoolean();
            }

            return false;
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



    public async Task<(byte[] Bytes, string ContentType)> DownloadLabelAsync(string labelUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            // The label URL is a SuperFrete-issued URL; download it with the
            // same authenticated client used for all SuperFrete calls.
            var response = await _httpClient.GetAsync(labelUrl, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("[Label] Download failed: status={Status} url={Url} body={Body}",
                    (int)response.StatusCode, labelUrl, body);
                throw new BusinessException(BusinessErrorMessage.SOMETHING_WENT_WRONG);
            }

            var bytes       = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/pdf";

            return (bytes, contentType);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "[Label] HTTP request exception for url={Url}", labelUrl);
            throw new BusinessException(BusinessErrorMessage.SOMETHING_WENT_WRONG);
        }
        catch (Exception)
        {
            throw;
        }
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
