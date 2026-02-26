using Domain.Utils;

namespace Domain.Enumerators;

public enum Operation
{
    #region SERVER EXCEPTIONS

    [EnumDescription("SERVER_EXCEPTION")]
    SERVER_EXCEPTION = 1,

    #endregion SERVER EXCEPTIONS



    #region CLIENT EXCEPTIONS

    [EnumDescription("VALIDATION_EXCEPTION")]
    VALIDATION_EXCEPTION = 2,

    [EnumDescription("AUTHENTICATION_EXCEPTION")]
    AUTHENTICATION_EXCEPTION = 3,

    [EnumDescription("FORBIDDEN_EXCEPTION")]
    FORBIDDEN_EXCEPTION = 4,

    [EnumDescription("BUSINESS_EXCEPTION")]
    BUSINESS_EXCEPTION = 5,

    [EnumDescription("MAIL_EXCEPTION")]
    MAIL_EXCEPTION = 6,

    [EnumDescription("PERSISTENCE_EXCEPTION")]
    PERSISTENCE_EXCEPTION = 7,

    [EnumDescription("GENERATE_EVIDENCE_FILES")]
    GENERATE_EVIDENCE_FILES = 8,

    [EnumDescription("ACCEPT_TERMS")]
    ACCEPT_TERMS = 9,

    [EnumDescription("RECEIVE_TOKEN")]
    RECEIVE_TOKEN = 10,

    [EnumDescription("SIGN_TOKEN")]
    SIGN_TOKEN = 11,

    #endregion CLIENT EXCEPTIONS
}
