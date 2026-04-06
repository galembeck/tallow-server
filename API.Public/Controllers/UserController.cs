using API.Public.Controllers._Base;
using API.Public.DTOs;
using API.Public.Filters;
using API.Public.Validators;
using Domain.Data.Entities;
using Domain.Enumerators;
using Domain.Exceptions;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Public.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(IUserService userService, IUserAddressService userAddressService) : _BaseController
{
    private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    private readonly IUserAddressService _userAddressService = userAddressService ?? throw new ArgumentNullException(nameof(userAddressService));

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] PrivateUserDTO body)
    {
        try
        {
            var securityInfo = GetSecurityInfo(Request);

            await new UserCreationValidator().ValidateAndThrowAsync(body);

            var model = await _userService.CreateAsync(PrivateUserDTO.DTOToModel(body), securityInfo);

            return Ok(PublicUserDTO.ModelToDTO(model));
        } catch (Exception e)
        {
            StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            throw;
        }
    }

    [AuthAttribute]
    [Filters.Authorize(ProfileType.CLIENT, ProfileType.ADMIN)]
    [HttpGet]
    public async Task<IActionResult> Me(CancellationToken cancellationToken = default)
    {
        var securityInfo = base.GetSecurityInfo(Request);

        User response = await _userService.GetUserAsync(Authenticated.User.Id, securityInfo, cancellationToken);

        return Ok(PublicUserDTO.ModelToDTO(response));
    }

    [HttpPut("me")]
    [AuthAttribute]
    [Filters.Authorize(ProfileType.CLIENT, ProfileType.ADMIN)]
    [ProducesResponseType(typeof(PublicUserDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateMe([FromForm] UpdateProfileDTO dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = Authenticated?.User?.Id;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                if (dto.Password != dto.PasswordConfirmation)
                    return StatusCode(StatusCodes.Status400BadRequest, "Passwords do not match.");

                if (dto.Password.Length < 8)
                    return StatusCode(StatusCodes.Status400BadRequest, "Password must be at least 8 characters.");
            }

            var user = await _userService.UpdateProfileAsync(
                userId,
                dto.Name,
                dto.Email,
                dto.Cellphone,
                dto.Document,
                dto.Password,
                dto.ReceiveEmailOffers,
                dto.ReceiveWhatsappOffers,
                dto.Avatar,
                cancellationToken);

            return Ok(PublicUserDTO.ModelToDTO(user));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
        }
    }

    #region .: USER ADDRESS(ES) :.

    [HttpPost("address")]
    [AuthAttribute]
    [Filters.AuthorizeAttribute(ProfileType.CLIENT, ProfileType.ADMIN)]
    public async Task<IActionResult> RegisterAddress([FromBody] RegisterAddressDTO body, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = Authenticated?.User.Id;

            await new AddressRegistrationValidator().ValidateAndThrowAsync(body);

            var address = RegisterAddressDTO.DTOToModel(body);
            var addressSaved = await _userAddressService.CreateAsync(address, userId);

            var response = AddressResponseDTO.ModelToDTO(addressSaved);

            response.CreatedBy = Authenticated?.User?.Name;
            response.UpdatedBy = Authenticated?.User?.Name;

            return Ok(response);
        }
        catch (Exception e)
        {
            StatusCode(StatusCodes.Status500InternalServerError, e.Message);

            throw;
        }
    }

    [HttpGet("address")]
    [AuthAttribute]
    [Filters.AuthorizeAttribute(ProfileType.CLIENT, ProfileType.ADMIN)]
    public async Task<IActionResult> GetUserAddresses(CancellationToken cancellationToken = default)
    {
        var userId = Authenticated?.User?.Id;

        var response = await _userAddressService.GetUserAddressesAsync(userId);

        return Ok(AddressResponseDTO.ModelToDTO(response));
    }

    [HttpGet("address/{addressId}")]
    [AuthAttribute]
    [Filters.AuthorizeAttribute(ProfileType.CLIENT, ProfileType.ADMIN)]
    public async Task<IActionResult> GetAddressById(string addressId, CancellationToken cancellationToken = default)
    {
        var response = await _userAddressService.GetByIdAsync(addressId);

        return Ok(AddressResponseDTO.ModelToDTO(response));
    }

    [HttpPut("address/{addressId}")]
    [AuthAttribute]
    [Filters.AuthorizeAttribute(ProfileType.CLIENT, ProfileType.ADMIN)]
    [ProducesResponseType(typeof(AddressResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAddress(string addressId, [FromForm] UpdateAddressDTO body, CancellationToken cancellationToken = default)
    {
        try
        {
            var actorId = Authenticated?.User?.Id;

            var validator = new AddressUpdateValidator();
            await validator.ValidateAndThrowAsync(body);

            var existing = await _userAddressService.GetByIdAsync(addressId, cancellationToken);
            if (existing == null)
                return NotFound();

            var updated = UpdateAddressDTO.ApplyToModel(body, existing);

            var saved = await _userAddressService.UpdateAsync(
                addressId,
                body.AddressTitle,
                body.ReceiverName,
                body.ReceiverLastname,
                body.ContactCellphone,
                body.Zipcode,
                body.Address,
                body.Number,
                body.Complement,
                body.Neighborhood,
                body.City,
                body.State,
                cancellationToken);

            return Ok(AddressResponseDTO.ModelToDTO(saved));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("address/{addressId}")]
    [AuthAttribute]
    [Filters.AuthorizeAttribute(ProfileType.CLIENT, ProfileType.ADMIN)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAddressById(string addressId, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = Authenticated?.User?.Id;

            var address = await _userAddressService.GetByIdAsync(addressId);
            if (address == null)
                throw new BusinessException(BusinessErrorMessage.ADDRESS_NOT_FOUND);

            await _userAddressService.DeleteAsync(address, userId);

            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }


    #endregion .: USER ADDRESS(ES) :.

    #region .: ADMIN: CLIENTS :.

    [HttpGet("admin/clients")]
    [AuthAttribute]
    [Filters.AuthorizeAttribute(ProfileType.ADMIN)]
    [ProducesResponseType(typeof(List<UserAdminSummaryDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllClients(CancellationToken cancellationToken = default)
    {
        try
        {
            var users = await _userService.GetAllByProfileTypeAsync(ProfileType.CLIENT, cancellationToken);
            return Ok(UserAdminSummaryDTO.ToDTO(users));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
        }
    }

    [HttpGet("admin/clients/{id}")]
    [AuthAttribute]
    [Filters.AuthorizeAttribute(ProfileType.ADMIN)]
    [ProducesResponseType(typeof(PublicUserDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetClientDetail(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userService.GetUserDetailByAdminAsync(id, cancellationToken);
            return Ok(PublicUserDTO.ModelToDTO(user));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
        }
    }

    [HttpPut("admin/clients/{id}")]
    [AuthAttribute]
    [Filters.AuthorizeAttribute(ProfileType.ADMIN)]
    [ProducesResponseType(typeof(PublicUserDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateClient(string id, [FromBody] UpdateUserAdminDTO dto, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Name) && string.IsNullOrWhiteSpace(dto.Email))
                return StatusCode(StatusCodes.Status400BadRequest, "At least one field (name or email) must be provided.");

            var user = await _userService.UpdateUserByAdminAsync(id, dto.Name, dto.Email, cancellationToken);
            return Ok(PublicUserDTO.ModelToDTO(user));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
        }
    }

    #endregion .: ADMIN: CLIENTS :.

    #region .: ADMIN: ADMINS :.

    [HttpGet("admin/admins")]
    [AuthAttribute]
    [Filters.AuthorizeAttribute(ProfileType.ADMIN)]
    [ProducesResponseType(typeof(List<UserAdminSummaryDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllAdmins(CancellationToken cancellationToken = default)
    {
        try
        {
            var users = await _userService.GetAllByProfileTypeAsync(ProfileType.ADMIN, cancellationToken);
            return Ok(UserAdminSummaryDTO.ToDTO(users));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
        }
    }

    [HttpGet("admin/admins/{id}")]
    [AuthAttribute]
    [Filters.AuthorizeAttribute(ProfileType.ADMIN)]
    [ProducesResponseType(typeof(PublicUserDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAdminDetail(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userService.GetUserDetailByAdminAsync(id, cancellationToken);
            return Ok(PublicUserDTO.ModelToDTO(user));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
        }
    }

    #endregion .: ADMIN: ADMINS :.

    #region .: ADMIN: PROFILE TYPE :.

    [HttpPut("admin/{id}/profile")]
    [AuthAttribute]
    [Filters.AuthorizeAttribute(ProfileType.ADMIN)]
    [ProducesResponseType(typeof(PublicUserDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangeProfileType(string id, [FromBody] ChangeProfileTypeDTO dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userService.ChangeUserProfileTypeAsync(id, dto.ProfileType, cancellationToken);
            return Ok(PublicUserDTO.ModelToDTO(user));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
        }
    }

    #endregion .: ADMIN: PROFILE TYPE :.
}
