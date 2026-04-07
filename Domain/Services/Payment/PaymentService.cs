using Domain.Data.Entities;
using Domain.Data.Models;
using Domain.Enumerators;
using Domain.Repository;
using System.Net.NetworkInformation;
using System.Text.Json;

namespace Domain.Services;

public class PaymentService(
    IPaymentRepository paymentRepository,
    IMercadoPagoService mercadoPagoService,
    IOrderRepository orderRepository) : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository = paymentRepository;
    private readonly IMercadoPagoService _mercadoPagoService = mercadoPagoService;
    private readonly IOrderRepository _orderRepository = orderRepository;

    public async Task<Payment> CreatePaymentAsync(
        string userId,
        string? orderId,
        MercadoPagoPaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        var mpResponse = await _mercadoPagoService.CreatePaymentAsync(request, cancellationToken);

        var order = orderId != null ? await _orderRepository.GetAsync(orderId, cancellationToken) : null;

        var payment = new Payment
        {
            UserId = userId,
            OrderId = orderId,

            MercadoPagoPaymentId = mpResponse.Id,
            MercadoPagoPaymentMethodId = mpResponse.PaymentMethodId,
            PaymentTypeId = mpResponse.PaymentTypeId,

            Status = MapPaymentStatus(mpResponse.Status),
            TransactionAmount = mpResponse.TransactionAmount,
            Installments = mpResponse.Installments,

            PaymentMethod = MapPaymentMethod(mpResponse.PaymentTypeId),

            CurrencyId = mpResponse.CurrencyId ?? "BRL",
            AuthorizationCode = mpResponse.AuthorizationCode,
            LiveMode = mpResponse.LiveMode,
            StatementDescriptor = mpResponse.StatementDescriptor,
            ShippingAmount = order?.ShippingAmount ?? mpResponse.ShippingAmount,

            ExternalReference = mpResponse.ExternalReference,
            StatusDetail = mpResponse.StatusDetail,

            DateApproved = mpResponse.DateApproved,
            DateLastUpdated = mpResponse.DateLastUpdated,
            DateOfExpiration = mpResponse.DateOfExpiration,

            PixQrCode = mpResponse.PointOfInteraction?.TransactionData?.QrCode,
            PixQrCodeBase64 = mpResponse.PointOfInteraction?.TransactionData?.QrCodeBase64,
            PixCopyPaste = mpResponse.PointOfInteraction?.TransactionData?.QrCode,

            BoletoUrl = mpResponse.PointOfInteraction?.TransactionData?.TicketUrl,
            BoletoBarcode = mpResponse.Barcode?.Content,

            RawMercadoPagoResponse = JsonSerializer.Serialize(mpResponse, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            })
        };

        await _paymentRepository.InsertAsync(payment);

        return payment;
    }

    public async Task<Payment?> GetPaymentByIdAsync(string paymentId, CancellationToken cancellationToken = default)
    {
        return await _paymentRepository.GetAsync(paymentId, cancellationToken);
    }

    public async Task<Payment?> GetPaymentDetailAsync(string paymentId, CancellationToken cancellationToken = default)
    {
        return await _paymentRepository.GetByIdWithRelationsAsync(paymentId, cancellationToken);
    }

    public async Task<List<Payment>> GetUserPaymentsAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _paymentRepository.GetByUserIdAsync(userId, cancellationToken);
    }

    public async Task<List<Payment>> GetAllPaymentsAsync(CancellationToken cancellationToken = default)
    {
        return await _paymentRepository.GetAllWithRelationsAsync(cancellationToken);
    }

    public async Task<Payment> UpdatePaymentStatusAsync(
        long mercadoPagoPaymentId,
        CancellationToken cancellationToken = default)
    {
        var mpResponse = await _mercadoPagoService.GetPaymentAsync(mercadoPagoPaymentId, cancellationToken);

        var payment = await _paymentRepository.GetByMercadoPagoIdAsync(mercadoPagoPaymentId, cancellationToken);

        if (payment == null)
            throw new Exception($"Payment {mercadoPagoPaymentId} not found");

        payment.Status = MapPaymentStatus(mpResponse.Status);
        payment.StatusDetail = mpResponse.StatusDetail;
        payment.DateApproved = mpResponse.DateApproved;
        payment.DateLastUpdated = mpResponse.DateLastUpdated;
        payment.RawMercadoPagoResponse = JsonSerializer.Serialize(mpResponse, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await _paymentRepository.UpdateAsync(payment);

        return payment;
    }

    #region .: HELPER METHODS :.

    /// <summary>
    /// Mapeia o status retornado pelo Mercado Pago (lowercase) para o enum PaymentStatus
    /// </summary>
    private PaymentStatus MapPaymentStatus(string? status)
    {
        if (string.IsNullOrEmpty(status))
            return PaymentStatus.PENDING;

        return status.ToLower() switch
        {
            "approved" => PaymentStatus.APPROVED,
            "pending" => PaymentStatus.PENDING,
            "rejected" => PaymentStatus.REJECTED,
            "in_process" => PaymentStatus.IN_PROCESS,
            "cancelled" => PaymentStatus.CANCELLED,
            "refunded" => PaymentStatus.REFUNDED,
            "charged_back" => PaymentStatus.CHARGED_BACK,
            _ => PaymentStatus.PENDING
        };
    }

    /// <summary>
    /// Mapeia o payment_type_id retornado pelo Mercado Pago para o enum PaymentMethod
    /// </summary>
    private PaymentMethod MapPaymentMethod(string? paymentTypeId)
    {
        if (string.IsNullOrEmpty(paymentTypeId))
            return PaymentMethod.CREDIT_CARD;

        return paymentTypeId.ToLower() switch
        {
            "credit_card" => PaymentMethod.CREDIT_CARD,
            "bank_transfer" => PaymentMethod.PIX,
            "ticket" => PaymentMethod.BOLETO,
            _ => PaymentMethod.CREDIT_CARD // Default
        };
    }

    #endregion
}