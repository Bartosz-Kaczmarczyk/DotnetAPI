namespace DotnetAPI.Models;

public class Post
{
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
}