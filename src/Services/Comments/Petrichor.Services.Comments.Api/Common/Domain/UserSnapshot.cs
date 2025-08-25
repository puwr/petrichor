namespace Petrichor.Services.Comments.Api.Common.Domain;

public class UserSnapshot
{
    public Guid UserId { get; private set; }
    public string UserName { get; private set; }

    public static UserSnapshot Create(Guid usedId, string userName)
    {
        return new UserSnapshot
        {
            UserId = usedId,
            UserName = userName
        };
    }
}