using Application.Abstractions;
using Domain.Entity;
using Infra.Persistence;
using Infra.Repository.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repository.Repositories
{
    public class CardRepository(TasksDbContext context) : BaseRepository<Card>(context), ICardRepository
    {
        public async Task<IReadOnlyList<Card>> GetByListIdAsync(Guid listId, CancellationToken cancellationToken = default)
        {
            return await DbContext.Cards
                .Where(c => c.ListId == listId)
                .ToListAsync(cancellationToken);
        }
    }
}
