using API.Public.DTOs._Base;
using Domain.Data.Entities;
using Domain.Enumerators;

namespace API.Public.DTOs;

public class PublicUserDTO : PublicBaseDTO<User>
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Cellphone { get; set; }
    public string Document { get; set; }
    public ProfileType? ProfileType { get; set; }



    public bool? ReceiveWhatsappOffers { get; set; }
    public bool? ReceiveEmailOffers { get; set; }



    public string? AvatarUrl { get; set; }



    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? LastAccessAt { get; set; }



    public PublicUserDTO(User o) : base(o)
    {
        if (o == null) return;

        Id = o.Id;
        Name = o.Name;
        Email = o.Email;
        Cellphone = o.Cellphone;
        Document = o.Document;
        ProfileType = o.ProfileType;
        ReceiveWhatsappOffers = o.ReceiveWhatsappOffers;
        ReceiveEmailOffers = o.ReceiveEmailOffers;
        AvatarUrl = o.AvatarUrl;
        CreatedAt = o.CreatedAt;
        LastAccessAt = o.LastAccessAt;
    }

    public static PublicUserDTO ModelToDTO(User o) => o == null ? null : new PublicUserDTO(o);

    public static List<PublicUserDTO> ModelToDTO(IEnumerable<User> users) => users.Select(user => new PublicUserDTO(user)).ToList();

    public static User DTOToModel(PublicUserDTO o)
    {
        if (o == null) return null;

        var model = new User()
        {
            Name = o.Name,
            Email = o.Email,
            Cellphone = o.Cellphone,
            Document = o.Document,
            ProfileType = o.ProfileType,
            ReceiveWhatsappOffers = o.ReceiveWhatsappOffers,
            ReceiveEmailOffers = o.ReceiveEmailOffers,
            AvatarUrl = o.AvatarUrl,
            CreatedAt = o.CreatedAt,
            LastAccessAt = o.LastAccessAt,
        };

        return o.InitializeInstance(model);
    }
}