using DotnetAPI.Dtos;
using DotnetAPI.Models;
using DotnetAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class PostController : ControllerBase
{
    IPostService _postService;
    //IMapper _mapper;

    public PostController(IConfiguration config, IPostService postService)
    {
        _postService = postService;
        //_mapper = new Mapper(new MapperConfiguration(cfg =>{
        //    cfg.CreateMap<UserToAddDto, User>();
        //}));
    }

    [HttpGet("Posts")]
    public IEnumerable<Post> GetPosts()
    {
        return _postService.GetPosts();
    }

    [HttpPost]
    public IActionResult AddPost(PostDto postAddDto)
    {
        var userId = _postService.GetIdFromClaim(User);
        _postService.AddPost(userId, postAddDto);
        return Ok();
    }

    [HttpGet("MyPosts")]
    public IEnumerable<Post> GetMyPosts()
    {
        var userId = _postService.GetIdFromClaim(User);
        return _postService.GetPostsByUserId(userId);
    }

    [HttpGet("Posts/User/{userId}")]
    public IEnumerable<Post> GetPostsByUser(int userId)
    {
        return _postService.GetPostsByUserId(userId);
    }

    [HttpGet("{postId}")]
    public Post GetPostById(int postId)
    {
        return _postService.GetPostById(postId);
    }

    [HttpPatch("Edit/{postId}")]
    public IActionResult EditPost(int postId, PostDto postUpdateDto)
    {
        var userId = _postService.GetIdFromClaim(User);
        _postService.EditPost(userId, postId, postUpdateDto);
        return Ok();
    }
    
    [HttpDelete("Delete/{postId}")]
    public IActionResult DeletePost(int postId)
    {
        var userId = _postService.GetIdFromClaim(User);
        _postService.DeletePost(userId, postId);
        return Ok();
    }

}