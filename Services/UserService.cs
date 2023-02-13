using System.Security.Claims;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;

namespace DotnetAPI.Services;

public interface IUserService
{
    User GetSingleUser(int userId);
    IEnumerable<User> GetUsers();
    void UpdateUser(ClaimsPrincipal user, UserUpdateDto userUpdateDto);
}

public class UserService : IUserService
{
    private readonly DataContextEF _entity;
    public UserService(IConfiguration config)
    {
        _entity = new DataContextEF(config);
    }

    public IEnumerable<User> GetUsers()
    {
        IEnumerable<User> users = _entity.Users.ToList<User>();
        return users;
    }

    public User GetSingleUser(int userId)
    {
        User? user = _entity.Users
            .Where(u => u.UserId == userId)
            .FirstOrDefault<User>();

        if (user != null)
        {
            return user;
        }

        throw new Exception("Failed to get user.");
    }

    public void UpdateUser(ClaimsPrincipal claim, UserUpdateDto userUpdateDto)
    {
        int userId;
        if (int.TryParse(claim.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value, out userId))
        {
            var user = GetSingleUser(userId);
            user.FirstName = userUpdateDto.FirstName;
            user.LastName = userUpdateDto.LastName;
            user.DateOfBirth = userUpdateDto.DateOfBirth;
            _entity.SaveChanges();
        }
        else
        {
        throw new Exception("Failed to update");
        }
    }
}