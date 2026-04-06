
using Domain.Utils;

namespace Domain.Enumerators;

public enum OrderStatus
{
    [EnumDescription("PENDING")]
    PENDING = 1,

    [EnumDescription("PAYMENT_PENDING")]
    PAYMENT_PENDING = 2,

    [EnumDescription("PAYMENT_APPROVED")]
    PAYMENT_APPROVED = 3,

    [EnumDescription("PREPARING")]
    PREPARING = 4,

    [EnumDescription("SHIPPED")]
    SHIPPED = 5,

    [EnumDescription("DELIVERED")]
    DELIVERED = 6,

    [EnumDescription("CANCELLED")]
    CANCELLED = 7,

    [EnumDescription("REFUNDED")]
    REFUNDED = 8
}
