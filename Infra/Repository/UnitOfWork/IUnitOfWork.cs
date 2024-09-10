using Infra.Repository.IRepositories;

namespace Infra.Repository.UnitOfWork
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        void Commit();
    }
}
