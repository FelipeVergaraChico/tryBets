using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using TryBets.Bets.Repository;
using TryBets.Bets.DTO;
using TryBets.Bets.Services;


namespace TryBets.Bets.Controllers;

[Route("[controller]")]
public class BetController : Controller
{
    private readonly IBetRepository _repository;
    private readonly IOddService _oddService;
    public BetController(IBetRepository repository, IOddService oddService)
    {
        _repository = repository;
        _oddService = oddService;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = "Client")]
    public async Task<IActionResult> Post([FromBody] BetDTORequest request)
    {
        try
        {
            var tk = HttpContext.User.Identity as ClaimsIdentity;
            var em = tk.FindFirst(ClaimTypes.Email)?.Value;
            return Created("", _repository.Post(request, em));
        }
        catch (Exception e)
        {
            
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpGet("{BetId}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = "Client")]
    public IActionResult Get(int BetId)
    {
        throw new NotImplementedException();
    }
}