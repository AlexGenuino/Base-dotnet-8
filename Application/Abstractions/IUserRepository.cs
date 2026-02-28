using Domain.Entity;

namespace Application.Abstractions;

public interface IUserRepository : IBaseRepository<User>
{
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default);
}
