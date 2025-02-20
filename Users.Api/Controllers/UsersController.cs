using Microsoft.AspNetCore.Mvc;
using Users.Api.Mapping;
using Users.Application.Services;
using Users.Contracts.Request;

namespace Users.Api.Controllers;

[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost(ApiEndpoints.Users.Create)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request,
        CancellationToken token)
    {
        var user = request.MapToUser();
        await _userService.CreateAsync(user, token);
        var response = user.MapToResponse();
        return CreatedAtAction(nameof(Get), new { id = user.Id }, response);
    }

    [HttpGet(ApiEndpoints.Users.Get)]
    [ResponseCache(Duration = 30, VaryByHeader = "Accept, Accept-Encoding", Location = ResponseCacheLocation.Any)]
    public async Task<IActionResult> Get([FromRoute] int id,
        CancellationToken token)
    {
        var user = await _userService.GetByIdAsync(id, token);
        if (user is null)
        {
            return NotFound();
        }

        var response = user.MapToResponse();
        return Ok(response);
    }

    [HttpGet(ApiEndpoints.Users.GetAll)]
    [ResponseCache(Duration = 30, VaryByQueryKeys = new[] { "date", "page", "pageSize" }, VaryByHeader = "Accept, Accept-Encoding", Location = ResponseCacheLocation.Any)]

    public async Task<IActionResult> GetAll([FromQuery] GetAllUsersRequest request,
        CancellationToken token)
    {
        var options = request.MapToOptions();
        var users = await _userService.GetAllAsync(options, token);
        var usersCount = await _userService.GetCountAsync(options.Date, token);
        var response = users.MapToResponse(request.Page, request.PageSize, usersCount);
        return Ok(response);
    }

    [HttpPut(ApiEndpoints.Users.Update)]
    public async Task<IActionResult> Update([FromRoute]int id,
        [FromBody] UpdateUserRequest request,
        CancellationToken token)
    {
        var user = request.MapToUser(id);
        var updated = await _userService.UpdateAsync(user, token);
        if (updated is null)
        {
            return NotFound();
        }

        var response = user.MapToResponse();

        return Ok(response);
    }

    [HttpDelete(ApiEndpoints.Users.Delete)]
    public async Task<IActionResult> Delete([FromRoute] int id,
        CancellationToken token)
    {
        var deleted = await _userService.DeleteByIdAsync(id, token);
        if (!deleted)
        {
            return NotFound();
        }

        return Ok();
    }
}
