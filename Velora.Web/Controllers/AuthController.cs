using Microsoft.AspNetCore.Mvc;
using MediatR;
using Velora.Application.Auth.Commands;

namespace Velora.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserCommand command)
    {
        var (succeeded, userId) = await _sender.Send(command);

        if (!succeeded)
        {
            return BadRequest("Kayıt işlemi başarısız oldu.");
        }

        return Ok(new { UserId = userId, Message = "Kayıt başarıyla tamamlandı." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserCommand command)
    {
        var (succeeded, token) = await _sender.Send(command);

        if (!succeeded)
        {
            return Unauthorized(new { Message = token }); // token contains error message in case of failure
        }

        return Ok(new { Token = token });
    }
}
