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
    //IMapper _mapper;

    public AuthController(IConfiguration config, IAuthService authService)
    {
        _authService = authService;
        //_mapper = new Mapper(new MapperConfiguration(cfg =>{
        //    cfg.CreateMap<UserToAddDto, User>();
        //}));
    }

    [HttpPost("Register")]
    public ActionResult RegisterUser(UserRegisterDto userRegisterDto)
    {
        _authService.RegisterUser(userRegisterDto);
        return Ok();
    }
    [HttpPost("login")]
    public ActionResult Login(UserLoginDto userLoginDto)
    {
        string token = _authService.GenerateJwt(userLoginDto);
        return Ok(token);
    }

}