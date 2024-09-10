using Domain.Entity;
using Infra.Repository.UnitOfWork;

namespace Infra.Repository.IRepositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
    }
}
