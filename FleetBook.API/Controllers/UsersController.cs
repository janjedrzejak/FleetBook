using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FleetBook.Models;
using FleetBook.Services;

namespace FleetBook.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(UserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Get all users
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<User>>> GetUsers()
    {
        _logger.LogInformation("👥 GET api/users called");
        var users = await _userService.GetUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUserById(int id)
    {
        _logger.LogInformation($"👥 GET api/users/{id} called");
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }
        return Ok(user);
    }

    /// <summary>
    /// Get users sorted by specified field
    /// </summary>
    [HttpGet("sorted/{sortBy}")]
    public async Task<ActionResult<List<User>>> GetUsersSorted(string sortBy, bool ascending = true)
    {
        _logger.LogInformation($"👥 GET api/users/sorted/{sortBy}?ascending={ascending} called");
        var users = await _userService.GetUsersSortedAsync(sortBy, ascending);
        return Ok(users);
    }

    /// <summary>
    /// Create a new user (admin only - create user without password initially)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<User>> CreateUser([FromBody] User user)
    {
        _logger.LogInformation($"👥 POST api/users called - Creating user: {user.Imie} {user.Nazwisko}");

        if (string.IsNullOrWhiteSpace(user.Imie) || string.IsNullOrWhiteSpace(user.Email))
        {
            return BadRequest(new { message = "Imie and Email are required" });
        }

        await _userService.AddUserAsync(user);
        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
    }

    /// <summary>
    /// Update an existing user
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateUser(int id, [FromBody] User user)
    {
        _logger.LogInformation($"👥 PUT api/users/{id} called - Updating user");

        if (id != user.Id)
        {
            return BadRequest(new { message = "ID mismatch" });
        }

        if (string.IsNullOrWhiteSpace(user.Imie) || string.IsNullOrWhiteSpace(user.Email))
        {
            return BadRequest(new { message = "Imie and Email are required" });
        }

        var existingUser = await _userService.GetUserByIdAsync(id);
        if (existingUser == null)
        {
            return NotFound(new { message = "User not found" });
        }

        await _userService.UpdateUserAsync(user);
        return NoContent();
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        _logger.LogInformation($"👥 DELETE api/users/{id} called");

        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        await _userService.DeleteUserAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Get all roles for a user
    /// </summary>
    [HttpGet("{id}/roles")]
    public async Task<ActionResult<List<Role>>> GetUserRoles(int id)
    {
        _logger.LogInformation($"👥 GET api/users/{id}/roles called");
        
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        var roles = await _userService.GetUserRolesAsync(id);
        return Ok(roles);
    }

    /// <summary>
    /// Update user roles (replaces all roles)
    /// </summary>
    [HttpPut("{id}/roles")]
    public async Task<ActionResult> UpdateUserRoles(int id, [FromBody] List<int> roleIds)
    {
        _logger.LogInformation($"👥 PUT api/users/{id}/roles called - Updating {roleIds.Count} roles");

        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            await _userService.UpdateUserRolesAsync(id, roleIds);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError($"👥 Error updating roles: {ex.Message}");
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Assign a single role to a user
    /// </summary>
    [HttpPost("{id}/roles/{roleId}")]
    public async Task<ActionResult> AssignRole(int id, int roleId)
    {
        _logger.LogInformation($"👥 POST api/users/{id}/roles/{roleId} called");

        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            await _userService.AssignRoleAsync(id, roleId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError($"👥 Error assigning role: {ex.Message}");
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Remove a role from a user
    /// </summary>
    [HttpDelete("{id}/roles/{roleId}")]
    public async Task<ActionResult> RemoveRole(int id, int roleId)
    {
        _logger.LogInformation($"👥 DELETE api/users/{id}/roles/{roleId} called");

        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        await _userService.RemoveRoleAsync(id, roleId);
        return NoContent();
    }

    /// <summary>
    /// Get all available roles
    /// </summary>
    [HttpGet("_/roles")]
    public async Task<ActionResult<List<Role>>> GetAllRoles()
    {
        _logger.LogInformation("👥 GET api/users/_/roles called");
        var roles = await _userService.GetAllRolesAsync();
        return Ok(roles);
    }
}