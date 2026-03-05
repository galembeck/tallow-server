using Domain.Utils;

namespace Domain.Enumerators;

public enum PaymentStatus
{
    [EnumDescription("PENDING")]
    PENDING = 1,

    [EnumDescription("APPROVED")]
    APPROVED = 2,

    [EnumDescription("REJECTED")]
    REJECTED = 3,

    [EnumDescription("IN_PROCESS")]
    IN_PROCESS = 4,

    [EnumDescription("CANCELLED")]
    CANCELLED = 5,

    [EnumDescription("REFUNDED")]
    REFUNDED = 6,

    [EnumDescription("CHARGED_BACK")]
    CHARGED_BACK = 7,
}
