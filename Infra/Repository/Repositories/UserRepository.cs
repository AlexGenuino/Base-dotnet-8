using Infra.Persistence;
using Infra.Repository.UnitOfWork;

namespace Infra.Repository.Repositories
{
    public class UserRepository(TasksDbContext context) : BaseRepository<User>(context), IUserRepository
    {
        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await DbContext.Users.AnyAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return await DbContext.Users.AnyAsync(u => u.Username == username, cancellationToken);
        }
    }
}
