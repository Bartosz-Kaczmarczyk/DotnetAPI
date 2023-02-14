using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Services;

public interface IAuthService
{
    void Activate(int userId, int activationKey);
    string GenerateJwt(UserLoginDto userLoginDto);
    int RegisterUser(UserRegisterDto userRegisterDto);
    void ResetPassword(int userId, int activationKey, PasswordChangeDto userDto);
    void ResetPasswordRequest(string nick);
}

public class AuthService : IAuthService
{
    private readonly IConfiguration _config;
    private readonly DataContextEF _entity;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IEmailSender _emailSender;
    public AuthService(IConfiguration config, IPasswordHasher<User> passwordHasher, IEmailSender emailSender)
    {
        _config = config;
        _entity = new DataContextEF(config);
        _passwordHasher = passwordHasher;
        _emailSender = emailSender;
    }

    public int RegisterUser(UserRegisterDto userRegisterDto)
    {
        var activationKey = GetActivationKey();
        var newUser = new User()
        {
            Nick = userRegisterDto.Nick,
            Email = userRegisterDto.Email,
            ActivationKey = activationKey
        };
        var hashedPassword = _passwordHasher.HashPassword(newUser, userRegisterDto.Password);

        newUser.PasswordHash = hashedPassword;
        _entity.Users.Add(newUser);
        _entity.SaveChanges();
        var user = _entity.Users
            .FirstOrDefault(u => u.Nick == newUser.Nick);
        var msg = $"http://localhost:5094/Auth/Activate/User/{user.UserId}/ActivationKey/{activationKey}";

        _emailSender.SendEmailAsync(newUser.Email, "Activation Key", msg);
        return activationKey;
    }

    private int GetActivationKey()
    {
        int _min = 1000;
        int _max = 9999;
        Random _rdm = new Random();
        return _rdm.Next(_min, _max);
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

        if (!user.IsActivated)
        {
            throw new Exception("User is not activated");
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

    public void Activate(int userId, int activationKey)
    {
        var user = _entity.Users
            .FirstOrDefault(u => u.UserId == userId);
        
        if (user != null && user.ActivationKey == activationKey)
        {
            user.IsActivated = true;
            _entity.SaveChanges();
        }
    }

    public void ResetPasswordRequest(string nick)
    {
        var activationKey = GetActivationKey();
        var user = _entity.Users.FirstOrDefault(u => u.Nick == nick);

        if (user != null)
        {
            user.ActivationKey = activationKey;
        }
        _entity.SaveChanges();

        string msg = $"http://localhost:5094/Auth/ResetPass/User/{user.UserId}/ActivationKey/{activationKey}";

        _emailSender.SendEmailAsync(user.Email, "Password reset", msg);
    }

    public void ResetPassword(int userId, int activationKey, PasswordChangeDto userDto)
    {
        
        var user = _entity.Users.FirstOrDefault(u => u.UserId == userId);
        
        if (user != null && user.ActivationKey == activationKey)
        {
            var hashedPassword = _passwordHasher.HashPassword(user, userDto.Password);
            user.PasswordHash = hashedPassword;
            _entity.SaveChanges();
        }
        else
        {
            throw new Exception("User not found");
        }
    
    }
}