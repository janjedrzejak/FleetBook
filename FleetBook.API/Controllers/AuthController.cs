using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FleetBook.Models;
using FleetBook.Services;

namespace FleetBook.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginRequest request)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var response = await _authService.LoginAsync(request.Email, request.Password);

    if (response == null)
        return Unauthorized(new { message = "Invalid email or password" });

    return Ok(response);
}


    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok(new { message = "Logout successful" });
    }
}
