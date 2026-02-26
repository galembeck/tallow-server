using Domain.Utils;

namespace Domain.Enumerators;

public enum SendTo
{
    [EnumDescription("EMAIL")]
    EMAIL = 1,

    [EnumDescription("CELLPHONE")]
    CELLPHONE = 2,
}
