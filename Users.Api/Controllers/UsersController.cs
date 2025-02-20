using Microsoft.AspNetCore.Mvc;
using Users.Api.Mapping;
using Users.Application.Services;
using Users.Contracts.Request;

namespace Users.Api.Controllers;

[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpPost(ApiEndpoints.Users.Create)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request,
        CancellationToken token)
    {
        _logger.LogInformation("Creating user with email: {Email}", request.Email);
        var user = request.MapToUser();
        await _userService.CreateAsync(user, token);
        var response = user.MapToResponse();
        _logger.LogInformation("User with email: {Email} created", request.Email);
        return CreatedAtAction(nameof(Get), new { id = user.Id }, response);
    }

    [HttpGet(ApiEndpoints.Users.Get)]
    [ResponseCache(Duration = 30, VaryByHeader = "Accept, Accept-Encoding", Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> Get([FromRoute] int id,
        CancellationToken token)
    {
        _logger.LogInformation("Getting user with ID: {Id}", id);
        var user = await _userService.GetByIdAsync(id, token);
        if (user is null)
        {
            _logger.LogInformation("Could not find any user with ID: {Id}", id);
            return NotFound();
        }

        var response = user.MapToResponse();
        _logger.LogInformation("User with ID: {Id} retrived", id);
        return Ok(response);
    }

    [HttpGet(ApiEndpoints.Users.GetAll)]
    [ResponseCache(Duration = 30, VaryByQueryKeys = new[] { "date", "page", "pageSize" }, VaryByHeader = "Accept, Accept-Encoding", Location = ResponseCacheLocation.Any)]

    public async Task<IActionResult> GetAll([FromQuery] GetAllUsersRequest request,
        CancellationToken token)
    {
        _logger.LogInformation("Getting all users from page: {page}, with pageSize: {pageSize}", request.Page, request.PageSize);
        var options = request.MapToOptions();
        var users = await _userService.GetAllAsync(options, token);
        var usersCount = await _userService.GetCountAsync(options.Date, token);
        var response = users.MapToResponse(request.Page, request.PageSize, usersCount);
        _logger.LogInformation("Finished retriving users");
        return Ok(response);
    }

    [HttpPut(ApiEndpoints.Users.Update)]
    public async Task<IActionResult> Update([FromRoute]int id,
        [FromBody] UpdateUserRequest request,
        CancellationToken token)
    {
        _logger.LogInformation("Updating user with ID: {Id}", id);
        var user = request.MapToUser(id);
        var updated = await _userService.UpdateAsync(user, token);
        if (updated is null)
        {
            _logger.LogInformation("Could not find any user with ID: {Id}", id);
            return NotFound();
        }

        var response = user.MapToResponse();
        _logger.LogInformation("user with ID: {Id} updated", id);
        return Ok(response);
    }

    [HttpDelete(ApiEndpoints.Users.Delete)]
    public async Task<IActionResult> Delete([FromRoute] int id,
        CancellationToken token)
    {
        _logger.LogInformation("Deleting user with ID: {Id}", id);

        var deleted = await _userService.DeleteByIdAsync(id, token);
        if (!deleted)
        {
            _logger.LogInformation("Could not find any user with the given Id: {Id}", id);
            return NotFound();
        }

        _logger.LogInformation("User with ID: {Id} deleted", id);
        return Ok();
    }
}
