using LocalCRM.Application.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocalCRM.API.Controllers;

[Authorize(Roles = "Administrator")]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetUsers()
    {
        return await _userService.GetUsersAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var result = await _userService.GetUserByIdAsync(id);
        if (result == null) return NotFound();
        return result;
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserCommand command)
    {
        try
        {
            var result = await _userService.CreateUserAsync(command);
            return CreatedAtAction(nameof(GetUser), new { id = result.UserId }, result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, UpdateUserCommand command)
    {
        if (id != command.UserId) return BadRequest();
        try { await _userService.UpdateUserAsync(command); }
        catch (Exception ex) { return NotFound(new { message = ex.Message }); }
        return NoContent();
    }

    [HttpPost("{id}/reset-password")]
    public async Task<IActionResult> ResetPassword(int id, [FromBody] string newPassword)
    {
        try { await _userService.ResetPasswordAsync(id, newPassword); }
        catch (Exception ex) { return NotFound(new { message = ex.Message }); }
        return NoContent();
    }
}
