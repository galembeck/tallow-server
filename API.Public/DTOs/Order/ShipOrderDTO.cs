namespace API.Public.DTOs;

public class ShipOrderDTO
{
    public int ServiceCode { get; set; }
    public float PackageWeight { get; set; }
    public float PackageHeight { get; set; }
    public float PackageWidth { get; set; }
    public float PackageLength { get; set; }
    public string? AdminNotes { get; set; }
}
