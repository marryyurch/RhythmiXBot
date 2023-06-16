using Amazon.DynamoDBv2.Model;
using AWS.DynamoDB.Bot.Clients;
using AWS.DynamoDB.Models;
using Microsoft.AspNetCore.Mvc;

namespace AWS.DynamoDB.Bot.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly DbClient _dbClient;
    public UsersController(DbClient dbClient)
    {
        _dbClient = dbClient;
    }

    [HttpGet("get-by-id")]
    public async Task<IActionResult> GetById(string userId)
    {
        var user = await _dbClient.GetById(userId);

        return user is not null ? Ok(user) : NotFound($"User with Id {userId} Does Not Exist");
    }

    [HttpGet("get-all-users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var user = await _dbClient.GetAllUsers();

        return Ok(user);
    }

    [HttpPost("create-user")]
    public async Task<IActionResult> CreateUser(User userRequest)
    {
        if (!await _dbClient.CreateUser(userRequest))
            return BadRequest($"User with Id {userRequest.ID} Already Exists");

        return Ok(userRequest);
    }

    [HttpDelete("delete-by-id")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        if (!await _dbClient.DeleteUser(userId))
            return NotFound($"User with Id {userId} Does Not Exist");

        return NoContent();
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser(User userRequest)
    {
        if (!await _dbClient.UpdateUser(userRequest))
            return NotFound($"User with Id {userRequest.ID} Does Not Exist");

        return Ok(userRequest);
    }
}