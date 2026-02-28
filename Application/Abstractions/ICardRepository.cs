using Domain.Entity;

namespace Application.Abstractions;

public interface ICardRepository : IBaseRepository<Card>
{
    Task<IReadOnlyList<Card>> GetByListIdAsync(Guid listId, CancellationToken cancellationToken = default);
}
