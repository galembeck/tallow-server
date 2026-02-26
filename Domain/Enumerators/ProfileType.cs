using Domain.Utils;

namespace Domain.Enumerators;

public enum ProfileType
{
    [EnumDescription("CPFL_ADMIN")]
    ADMIN = 1,

    [EnumDescription("CLIENT")]
    CLIENT = 2,
}
