using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Services;

public interface IAuthService
{
    string GenerateJwt(UserLoginDto userLoginDto);
    void RegisterUser(UserRegisterDto userRegisterDto);
}

public class AuthService : IAuthService
{
    private readonly IConfiguration _config;
    private readonly DataContextEF _entity;
    private readonly IPasswordHasher<User> _passwordHasher;
    public AuthService(IConfiguration config, IPasswordHasher<User> passwordHasher)
    {
        _config = config;
        _entity = new DataContextEF(config);
        _passwordHasher = passwordHasher;
    }

    public void RegisterUser(UserRegisterDto userRegisterDto)
    {
        var newUser = new User()
        {
            Nick = userRegisterDto.Nick,
            Email = userRegisterDto.Email
        };
        var hashedPassword = _passwordHasher.HashPassword(newUser, userRegisterDto.Password);

        newUser.PasswordHash = hashedPassword;
        _entity.Users.Add(newUser);
        _entity.SaveChanges();
    }

    public string GenerateJwt(UserLoginDto userLoginDto)
    {
        var user = _entity.Users
            .FirstOrDefault(u => u.Nick == userLoginDto.Nick);

        if (user is null)
        {
            throw new Exception("Invalid username or password");
            // throw new BadRequestException("Invalid username or password");
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, userLoginDto.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new Exception("Invalid username or password");
            // throw new BadRequestException("Invalid username or password");
        }

        var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),

            };

        var tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;
        var tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKeyString != null ? tokenKeyString: ""));
        var credentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha256);

        SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credentials,
                Expires = DateTime.Now.AddDays(1)
            };

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

        SecurityToken token = tokenHandler.CreateToken(descriptor);

        return tokenHandler.WriteToken(token);
    }
}