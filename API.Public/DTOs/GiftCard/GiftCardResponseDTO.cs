using Domain.Data.Entities;
using Domain.Enumerators;

namespace API.Public.DTOs;

public class GiftCardResponseDTO
{
    public string Id { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTimeOffset PurchasedAt { get; set; }
    public DateTime? UsedAt { get; set; }
    public string? UsedOnOrderId { get; set; }

    // Payment info (for pending PIX/Boleto state)
    public string? PixQrCode { get; set; }
    public string? PixQrCodeBase64 { get; set; }
    public string? PixCopyPaste { get; set; }
    public DateTime? PaymentExpiresAt { get; set; }

    // Admin-only fields (populated when requested by admin)
    public string? BuyerName { get; set; }
    public string? BuyerEmail { get; set; }

    public static GiftCardResponseDTO ToDTO(GiftCard g, bool includeUserInfo = false) => new()
    {
        Id              = g.Id,
        Amount          = g.Amount,
        Status          = g.Status.ToString(),
        ExpiresAt       = g.ExpiresAt,
        PurchasedAt     = g.CreatedAt,
        UsedAt          = g.UsedAt,
        UsedOnOrderId   = g.UsedOnOrderId,
        PixQrCode       = g.Status == GiftCardStatus.PENDING ? g.Payment?.PixQrCode : null,
        PixQrCodeBase64 = g.Status == GiftCardStatus.PENDING ? g.Payment?.PixQrCodeBase64 : null,
        PixCopyPaste    = g.Status == GiftCardStatus.PENDING ? g.Payment?.PixCopyPaste : null,
        PaymentExpiresAt = g.Status == GiftCardStatus.PENDING ? g.Payment?.DateOfExpiration : null,
        BuyerName       = includeUserInfo ? g.User?.Name : null,
        BuyerEmail      = includeUserInfo ? g.User?.Email : null,
    };
}
