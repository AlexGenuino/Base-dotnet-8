using Application.Abstractions;
using Infra.Persistence;
using Infra.Repository.Repositories;

namespace Infra.Repository.UnitOfWork
{
    public class UnitOfWork(TasksDbContext context, IUserRepository userRepository, ICardRepository cardRepository) : IUnitOfWork
    {
        private readonly TasksDbContext _context = context;

        public IUserRepository UserRepository => userRepository;
        public ICardRepository CardRepository => cardRepository;

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
