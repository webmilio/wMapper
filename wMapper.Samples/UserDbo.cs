namespace WMapper.Samples;

public class UserDbo
{
    public int Id { get; set; }

    public required string Username { get; set; }
    public required string Email { get; set; }

    public required string Password { get; set; }

    public DateTime CreatedOn { get; set; }
}
