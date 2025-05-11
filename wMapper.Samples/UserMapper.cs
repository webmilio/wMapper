namespace wMapper.Samples;

public class UserMapper : IAdapter<UserDbo, UserDto>
{
    public UserDto Adapt(UserDbo src)
    {
        return new()
        {
            Id = src.Id,
            Username = src.Username,
            CreatedOn = src.CreatedOn
        };
    }
}
