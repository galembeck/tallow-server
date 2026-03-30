using API.Public.Controllers._Base;
using API.Public.DTOs;
using API.Public.Filters;
using API.Public.Validators;
using Domain.Data.Entities;
using Domain.Enumerators;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Public.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(IUserService userService) : _BaseController
{
    private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));

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

    // ─── ADMIN: CLIENTS ──────────────────────────────────────────────────────

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

    // ─── ADMIN: ADMINS ────────────────────────────────────────────────────────

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

    // ─── ADMIN: PROFILE TYPE ─────────────────────────────────────────────────

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
}
