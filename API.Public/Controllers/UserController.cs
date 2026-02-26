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
}
