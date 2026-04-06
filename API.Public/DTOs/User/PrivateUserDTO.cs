using API.Public.DTOs._Base;
using Domain.Data.Entities;
using Domain.Enumerators;
using Domain.Utils;
using System.Globalization;

namespace API.Public.DTOs;

public class PrivateUserDTO : PrivateBaseDTO<User>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cellphone { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;
    public ProfileType? ProfileType { get; set; }



    public string Password { get; set; } = string.Empty;



    public bool? ReceiveWhatsappOffers { get; set; }
    public bool? ReceiveEmailOffers { get; set; }



    public DateTimeOffset? LastAccessAt { get; set; }



    public string HashId { get; set; } = string.Empty;



    #region .: METHODS :.

    public PrivateUserDTO(User o) : base(o)
    {
        if (o == null) return;

        Name = o.Name;
        Email = o.Email;
        Cellphone = o.Cellphone;
        Document = o.Document;
        ProfileType = o.ProfileType;
        Password = o.Password;
        ReceiveWhatsappOffers = o.ReceiveWhatsappOffers;
        ReceiveEmailOffers = o.ReceiveEmailOffers;
        LastAccessAt = o.LastAccessAt;
        HashId = o.HashId;
    }

    public PrivateUserDTO() { }

    public static PrivateUserDTO? ModelToDTO(User o) => o == null ? null : new PrivateUserDTO(o);

    public static User? DTOToModel(PrivateUserDTO o)
    {
        if (o == null) return null;

        var model = new User
        {
            Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(o.Name.ToLower().Trim()),
            Email = o.Email.ToLower().Trim(),
            Cellphone = o.Cellphone,
            Document = StringUtil.Slugify(o.Document.Trim()),
            ProfileType = Domain.Enumerators.ProfileType.CLIENT,
            Password = o.Password.Trim(),
            ReceiveWhatsappOffers = o.ReceiveWhatsappOffers,
            ReceiveEmailOffers = o.ReceiveEmailOffers,
            LastAccessAt = o.LastAccessAt,
            HashId = o.HashId,
        };

        return o.InitializeInstance(model);
    }

    public static List<PrivateUserDTO>? UserListToPrivateUserDTOList(List<User>? userList)
    {
        if (userList is null || userList.Count == 0) return null;

        List<PrivateUserDTO> privateUserList = new();

        foreach (User user in userList)
        {
            var dto = ModelToDTO(user);
            if (dto != null)
            {
                privateUserList.Add(dto);
            }
        }

        return privateUserList;
    }

    public static PrivateUserDTO? ModelToDTOWithoutBase(User o)
    {
        if (o == null) return null;

        var DTO = new PrivateUserDTO();
        DTO.MapFrom(o);

        return DTO;
    }

    private void MapFrom(User o)
    {
        Name = o.Name;
        Email = o.Email;
        Cellphone = o.Cellphone;
        Document = o.Document;
        ProfileType = o.ProfileType;
        ReceiveWhatsappOffers = o.ReceiveWhatsappOffers;
        ReceiveEmailOffers = o.ReceiveEmailOffers;
    }

    #endregion .: METHODS :.
}
