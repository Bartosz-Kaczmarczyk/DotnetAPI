using System.Security.Claims;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using DotnetAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    IUserService _userService;
    //IMapper _mapper;

    public UserController(IConfiguration config, IUserService userService)
    {
        _userService = userService;
        //_mapper = new Mapper(new MapperConfiguration(cfg =>{
        //    cfg.CreateMap<UserToAddDto, User>();
        //}));
    }

    [HttpGet]
    public IEnumerable<User> GetUsers()
    {
        // IEnumerable<User> users = _userService.GetUsers();
        // return users;
        return _userService.GetUsers();
    }

    [HttpGet("{userId}")]
    public User GetSingleUser(int userId)
    {
        return _userService.GetSingleUser(userId);
    }

    
    [HttpPatch("update")]
    public IActionResult UpdateUser(UserUpdateDto userUpdateDto)
    {
        _userService.UpdateUser(User, userUpdateDto);

        return Ok();
    }

}