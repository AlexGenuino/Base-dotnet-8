using Infra.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infra.Repository.UnitOfWork
{
    public class BaseRepository<T>(TasksDbContext context) : IBaseRepository<T> where T : class
    {
        private readonly TasksDbContext _context = context;

        public async Task<T> Create(T command)
        {
            await _context.Set<T>().AddAsync(command);
            return command;
        }

        public Task Delete(Guid Id)
        {
            _context.Remove(Id);
            return Task.CompletedTask;
        }

        public async Task<T?> Get(Expression<Func<T, bool>> expression)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(expression);
        }

        public IEnumerable<T> GetAll()
        {
            return [.. _context.Set<T>().ToList()];   
        }

        public async Task<T> Update(T commandUpdate)
        {
            _context.Set<T>().Update(commandUpdate);
            return commandUpdate;
        }
    }
}
