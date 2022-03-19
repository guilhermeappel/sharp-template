using Appel.SharpTemplate.API.Application.Interfaces;
using Appel.SharpTemplate.API.Application.Models;
using Appel.SharpTemplate.Domain.Entities;
using Appel.SharpTemplate.Infrastructure.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Appel.SharpTemplate.API.Controllers.API;

public class UsersController : BaseController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("authenticate")]
    public async Task<IActionResult> AuthenticateAsync([FromBody] UserAuthenticateViewModel user)
    {
        var token = await _userService.AuthenticateAsync(user);

        return Ok(token);
    }

    [HttpPut]
    [Route("edit")]
    public async Task<IActionResult> EditAsync([FromBody] UserProfileViewModel user)
    {
        var userTokenId = int.Parse(User.Identity?.Name ?? string.Empty);

        if (user.Id != userTokenId && !User.IsInRole(UserRole.Admin.ToString()))
        {
            return Forbid();
        }

        await _userService.UpdateAsync(user);

        return Ok();
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] UserRegisterViewModel user)
    {
        await _userService.RegisterAsync(user);

        return Ok();
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("forgot-password")]
    public async Task<IActionResult> ForgotPasswordAsync([FromBody] string email)
    {
        await _userService.SendForgotPasswordEmailAsync(email);

        return Ok();
    }

    [HttpPatch]
    [Route("change-password")]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] UserChangePasswordViewModel user)
    {
        await _userService.ChangePasswordAsync(user);

        return Ok();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetUserByIdAsync(int id)
    {
        var userTokenId = int.Parse(User.Identity?.Name ?? string.Empty);

        if (id != userTokenId && !User.IsInRole(UserRole.Admin.ToString()))
        {
            return Forbid();
        }

        var user = await _userService.GetByIdAsync(id);

        return Ok(user);
    }

    [HttpGet]
    [AuthorizeRoles(UserRole.Admin)]
    public async Task<IActionResult> GetAllUsersAsync()
    {
        var users = await _userService.GetAsync();

        return Ok(users);
    }
}
