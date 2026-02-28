using Application.Abstractions;
using Infra.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infra.Repository.UnitOfWork
{
    public class BaseRepository<T>(TasksDbContext context) : IBaseRepository<T> where T : class
    {
        private readonly TasksDbContext _context = context;
        protected TasksDbContext DbContext => _context;

        public async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _context.Set<T>().AddAsync(entity, cancellationToken);
            return entity;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Set<T>().FindAsync([id], cancellationToken);
            if (entity is not null)
                _context.Set<T>().Remove(entity);
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(expression, cancellationToken);
        }

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().FindAsync([id], cancellationToken);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().ToListAsync(cancellationToken);
        }

        public Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _context.Set<T>().Update(entity);
            return Task.FromResult(entity);
        }
    }
}
