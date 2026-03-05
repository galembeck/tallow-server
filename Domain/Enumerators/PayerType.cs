using Domain.Utils;

namespace Domain.Enumerators;

public enum PayerType
{
    [EnumDescription("customer")]
    customer,

    [EnumDescription("guest")]
    guest
}
