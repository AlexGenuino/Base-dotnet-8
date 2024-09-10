using System.Linq.Expressions;

namespace Infra.Repository.UnitOfWork
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T?> Get(Expression<Func<T, bool>> expression);
        IEnumerable<T> GetAll();
        Task<T> Create(T command);
        Task<T> Update(T commandUpdate);
        Task Delete(Guid Id);
    }
}
