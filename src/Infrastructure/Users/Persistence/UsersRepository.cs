using Application.Common.Interfaces;
using Domain.Users;
using Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Users.Persistence;

public class UsersRepository(PetrichorDbContext dbContext) : IUsersRepository
{
    private readonly PetrichorDbContext _dbContext = dbContext;

    public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        return user;
    }
}