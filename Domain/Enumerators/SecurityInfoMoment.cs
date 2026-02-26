using Domain.Utils;

namespace Domain.Enumerators;

public enum SecurityInfoMoment
{
    [EnumDescription("REGISTER")]
    REGISTER = 1,

    [EnumDescription("LOGIN")]
    LOGIN = 2,
}