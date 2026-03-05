using Domain.Enumerators;
using System.Text.Json.Serialization;

namespace API.Public.DTOs;

public class PaymentResponseDTO
{
    public string Id { get; set; }
    public string? OrderId { get; set; }
    public long? MercadoPagoPaymentId { get; set; }

    public string Status { get; set; }
    public string? StatusDetail { get; set; }
    public string PaymentMethod { get; set; }
    public string PaymentMethodId { get; set; }
    public string? PaymentTypeId { get; set; }

    public decimal TransactionAmount { get; set; }
    public int? Installments { get; set; }
    public string? CurrencyId { get; set; }
    public decimal? ShippingAmount { get; set; }

    public string? AuthorizationCode { get; set; }
    public bool? LiveMode { get; set; }
    public string? StatementDescriptor { get; set; }
    public string? ExternalReference { get; set; }

    public DateTimeOffset DateCreated { get; set; }
    public DateTime? DateApproved { get; set; }
    public DateTime? DateLastUpdated { get; set; }
    public DateTime? DateOfExpiration { get; set; }

    public string? PixQrCode { get; set; }
    public string? PixQrCodeBase64 { get; set; }
    public string? PixCopyPaste { get; set; }

    public string? BoletoUrl { get; set; }
    public string? BoletoBarcode { get; set; }

    public static PaymentResponseDTO ToDTO(Domain.Data.Entities.Payment payment)
    {
        return new PaymentResponseDTO
        {
            Id = payment.Id,
            OrderId = payment.OrderId,
            MercadoPagoPaymentId = payment.MercadoPagoPaymentId,

            Status = payment.Status.ToString(),
            StatusDetail = payment.StatusDetail,
            PaymentMethod = payment.PaymentMethod.ToString(),
            PaymentMethodId = payment.MercadoPagoPaymentMethodId,
            PaymentTypeId = payment.PaymentTypeId,

            TransactionAmount = payment.TransactionAmount,
            Installments = payment.Installments,
            CurrencyId = payment.CurrencyId,
            ShippingAmount = payment.ShippingAmount,

            AuthorizationCode = payment.AuthorizationCode,
            LiveMode = payment.LiveMode,
            StatementDescriptor = payment.StatementDescriptor,
            ExternalReference = payment.ExternalReference,

            DateCreated = payment.CreatedAt,
            DateApproved = payment.DateApproved,
            DateLastUpdated = payment.DateLastUpdated,
            DateOfExpiration = payment.DateOfExpiration,

            PixQrCode = payment.PixQrCode,
            PixQrCodeBase64 = payment.PixQrCodeBase64,
            PixCopyPaste = payment.PixCopyPaste,

            BoletoUrl = payment.BoletoUrl,
            BoletoBarcode = payment.BoletoBarcode
        };
    }
}