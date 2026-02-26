using Domain.Utils;

namespace Domain.Enumerators;

public enum AuthenticationErrorMessage
{
    [EnumDescription("UNAUTHORIZED")]
    UNAUTHORIZED = 1,

    [EnumDescription("TOKEN_EXPIRED")]
    TOKEN_EXPIRED = 2,

    [EnumDescription("ACCESSTOKEN_NOT_FOUND")]
    ACCESSTOKEN_NOT_FOUND = 3,

    [EnumDescription("REFRESHTOKEN_NOT_FOUND")]
    REFRESHTOKEN_NOT_FOUND = 4,

    [EnumDescription("INVALID_TOKEN")]
    INVALID_TOKEN = 5
}
