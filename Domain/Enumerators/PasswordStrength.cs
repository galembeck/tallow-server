using Domain.Utils;

namespace Domain.Enumerators;

public enum PasswordStrength
{
    [EnumDescription("VeryWeak")]
    VeryWeak = 1,

    [EnumDescription("Weak")]
    Weak = 2,

    [EnumDescription("Medium")]
    Medium = 3,

    [EnumDescription("Strong")]
    Strong = 4,

    [EnumDescription("VeryStrong")]
    VeryStrong = 5,
}
