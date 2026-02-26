namespace API.Public.DTOs;

public sealed record AuthenticateDTO
{
    public string Email { get; set; }
    public string Password { get; set; }
    //public string idHash { get; set; }
}
