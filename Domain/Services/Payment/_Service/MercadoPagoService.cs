using Domain.Constants;
using Domain.Data.Models;
using Domain.Utils.Constants;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Domain.Services;

public class MercadoPagoService : IMercadoPagoService
{
    private readonly HttpClient _httpClient;

    public MercadoPagoService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        var settings = Constant.Settings.PaymentServiceSettings;

        _httpClient.BaseAddress = new Uri(settings.Endpoint);
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", settings.AccessToken);
        _httpClient.DefaultRequestHeaders.Add("X-Idempotency-Key", Guid.NewGuid().ToString());
    }

    public async Task<MercadoPagoPaymentResponse> CreatePaymentAsync(
        MercadoPagoPaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        var jsonContent = JsonSerializer.Serialize(request, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/v1/payments", content, cancellationToken);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Error while creating Mercado Pago payment: {responseContent}");
        }

        var paymentResponse = JsonSerializer.Deserialize<MercadoPagoPaymentResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return paymentResponse!;
    }

    public async Task<MercadoPagoPaymentResponse> GetPaymentAsync(
        long paymentId,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/v1/payments/{paymentId}", cancellationToken);

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Error while fetching Mercado Pago payment: {responseContent}");
        }

        var paymentResponse = JsonSerializer.Deserialize<MercadoPagoPaymentResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return paymentResponse!;
    }
}