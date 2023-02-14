namespace DotnetAPI.Models;

public class User
{
    public int UserId { get; set; }
    public string Nick { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool IsAdmin { get; set; } = false;
    public int ActivationKey { get; set; }
    public bool IsActivated { get; set; }
}