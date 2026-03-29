using Domain.Utils;

namespace Domain.Enumerators;

public enum PaymentMethod
{
    [EnumDescription("CREDIT_CARD")]
    CREDIT_CARD = 1,

    [EnumDescription("PIX")]
    PIX = 2,

    [EnumDescription("BOLETO")]
    BOLETO = 3,
}
