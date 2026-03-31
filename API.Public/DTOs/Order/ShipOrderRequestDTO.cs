namespace API.Public.DTOs;

public class ShipOrderRequestDTO
{
    /// <summary>
    /// SuperFrete service ID: 1=PAC, 2=SEDEX, 17=Mini Envios, 3=Jadlog, 31=Loggi
    /// </summary>
    public int ServiceId { get; set; }
}
