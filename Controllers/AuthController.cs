using DotnetAPI.Dtos;
using DotnetAPI.Models;
using DotnetAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    IAuthService _authService;

    public AuthController(IConfiguration config, IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("Register")]
    public IActionResult RegisterUser(UserRegisterDto userRegisterDto)
    {
        var activationKey = _authService.RegisterUser(userRegisterDto);
        return Ok(activationKey);
    }

    [HttpPost("Activate/User/{userId}/ActivationKey/{activationKey}")]
    public IActionResult Activate(int userId, int activationKey)
    {
        _authService.Activate(userId, activationKey);
        return Ok();
    }

    [HttpPost("login")]
    public IActionResult Login(UserLoginDto userLoginDto)
    {
        string token = _authService.GenerateJwt(userLoginDto);
        return Ok(token);
    }

    [HttpPost("ResetPassword/{nick}")]
    public IActionResult ResetPasswordRequest(string nick)
    {
        _authService.ResetPasswordRequest(nick);
        return Ok();
    }

    [HttpPatch("ResetPassword/User/{userId}/ActivationKey/{activationKey}")]
    public IActionResult ResetPassword(int userId, int activationKey, PasswordChangeDto userDto)
    {
        _authService.ResetPassword(userId, activationKey, userDto);
        return Ok();
    }

}