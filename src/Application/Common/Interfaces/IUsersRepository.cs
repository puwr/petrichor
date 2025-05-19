using Domain.Users;

namespace Application.Common.Interfaces;

public interface IUsersRepository
{
    Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
}