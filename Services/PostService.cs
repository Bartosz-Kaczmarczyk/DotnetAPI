using System.Security.Claims;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;

namespace DotnetAPI.Services;

public interface IPostService
{
    public int GetIdFromClaim(ClaimsPrincipal user);
    public IEnumerable<Post> GetPosts();
    void AddPost(int userId, PostDto postAddDto);
    IEnumerable<Post> GetPostsByUserId(int userId);
    Post GetPostById(int postId);
    void EditPost(int userId, int postId, PostDto postUpdateDto);
    void DeletePost(int userId, int postId);
}

public class PostService : IPostService
{
    private readonly DataContextEF _entity;
    public PostService(IConfiguration config)
    {
        _entity = new DataContextEF(config);
    }

    public int GetIdFromClaim(ClaimsPrincipal user)
    {
        int userId;
        if (int.TryParse(user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value, out userId))
        {
           return userId; 
        }
        else
        {
            throw new Exception("User not found");
        }  
    }

    private bool IsUserAdmin(int userId)
    {
        var user = _entity.Users.FirstOrDefault(u => u.UserId == userId);
        if (user != null)
        {
            if (user.IsAdmin)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            throw new Exception("User not found");
        }
    }

    public IEnumerable<Post> GetPosts()
    {
        var posts = _entity.Posts.ToList<Post>();
        return posts;
    }
    public void AddPost(int userId, PostDto postAddDto)
    { 
        var post = new Post();
        post.UserId = userId;
        post.Title = postAddDto.Title;
        post.Body = postAddDto.Body;
        post.Created = DateTime.Now;
        post.Modified = DateTime.Now;
        _entity.Posts.Add(post);
        _entity.SaveChanges();    
    }

    public IEnumerable<Post> GetPostsByUserId(int userId)
    {
        IEnumerable<Post> myPosts = _entity.Posts.Where(p => p.UserId == userId);
        return myPosts;      
    }

    public Post GetPostById(int postId)
    {
        var post = _entity.Posts.FirstOrDefault(p => p.PostId == postId);
        if (post != null) 
        {
            return post;
        }
        else
        {
            throw new Exception("Post not found");
        }
    }

    public void EditPost(int userId, int postId, PostDto postUpdateDto)
    {
        var post = GetPostById(postId);
        if (IsUserAdmin(userId) || post.UserId == userId)
        {
            post.Title = postUpdateDto.Title;
            post.Body = postUpdateDto.Body;
            post.Modified = DateTime.Now;
            _entity.SaveChanges();
        }
        else
        {
            throw new Exception("Failed to edit post");
        }
        
    }

    public void DeletePost(int userId, int postId)
    {
        var post = GetPostById(postId);
        if (IsUserAdmin(userId) || post.UserId == userId)
        {
            _entity.Remove(post);
            _entity.SaveChanges();
        }
        else
        {
            throw new Exception("Failed to delete post");
        }
    }




    // public User GetSingleUserPosts(int userId)
    // {
    //     User? user = _entity.Users
    //         .Where(u => u.UserId == userId)
    //         .FirstOrDefault<User>();

    //     if (user != null)
    //     {
    //         return user;
    //     }

    //     throw new Exception("Failed to get user.");
    // }

    // public void UpdateUser(ClaimsPrincipal claim, UserUpdateDto userUpdateDto)
    // {
    //     int userId;
    //     if (int.TryParse(claim.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value, out userId))
    //     {
    //         var user = GetSingleUser(userId);
    //         user.FirstName = userUpdateDto.FirstName;
    //         user.LastName = userUpdateDto.LastName;
    //         user.DateOfBirth = userUpdateDto.DateOfBirth;
    //         _entity.SaveChanges();
    //     }
    //     else
    //     {
    //     throw new Exception("Failed to update");
    //     }
    // }
}
