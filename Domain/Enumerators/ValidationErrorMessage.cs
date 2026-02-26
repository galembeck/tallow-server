using Domain.Utils;

namespace Domain.Enumerators;

public enum ValidationErrorMessage
{
    [EnumDescription("INVALID_SCHEMA")]
    INVALID_SCHEMA = 1,

    [EnumDescription("INVALID_PARAMETER")]
    INVALID_PARAMETER = 2,

    [EnumDescription("INVALID_ID")]
    INVALID_ID = 3,

    [EnumDescription("INVALID_NAME")]
    INVALID_NAME = 4,

    [EnumDescription("INVALID_EMAIL")]
    INVALID_EMAIL = 5,

    [EnumDescription("INVALID_PASSWORD")]
    INVALID_PASSWORD = 6,

    [EnumDescription("INVALID_RECAPTCHA")]
    INVALID_RECAPTCHA = 7,

    [EnumDescription("INVALID_DOCUMENT")]
    INVALID_DOCUMENT = 8,

    [EnumDescription("INVALID_PROFILE_TYPE")]
    INVALID_PROFILE_TYPE = 9,

    [EnumDescription("ALREADY_VALIDATED_OR_EXPIRATED")]
    ALREADY_VALIDATED_OR_EXPIRATED = 10,

    [EnumDescription("INVALID_TOKEN")]
    INVALID_TOKEN = 11,

    [EnumDescription("INVALID_HEADER_TOKEN")]
    INVALID_HEADER_TOKEN = 12,

    [EnumDescription("INVALID_EMAIL_OR_TOKEN")]
    INVALID_EMAIL_OR_TOKEN = 13,

    [EnumDescription("TOKEN_ALREADY_SENT")]
    TOKEN_ALREADY_SENT = 14,

    [EnumDescription("FORBIDDEN_ROLE")]
    FORBIDDEN_ROLE = 15
}
