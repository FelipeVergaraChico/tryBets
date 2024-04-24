using System;
using Microsoft.AspNetCore.Mvc;
using TryBets.Users.Repository;
using TryBets.Users.Services;
using TryBets.Users.Models;
using TryBets.Users.DTO;

namespace TryBets.Users.Controllers;

[Route("[controller]")]
public class UserController : Controller
{
    private readonly IUserRepository _repository;
    public UserController(IUserRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("signup")]
    public IActionResult Post([FromBody] User user)
    {
        try
        {
            User u = _repository.Post(user);
            string tk = new TokenManager().Generate(u);
            AuthDTOResponse res = new AuthDTOResponse {
                Token = tk,
            };
            return Created("", res);
        }
        catch (Exception e)
        {
            
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] AuthDTORequest login)
    {
        try
        {
            User u = _repository.Login(login);
            string tk = new TokenManager().Generate(u);
            AuthDTOResponse res = new AuthDTOResponse {
                Token = tk,
            };
            return Ok(res);
        }
        catch (Exception e)
        {
            
            return BadRequest(new { message = e.Message });
        }
    }
}