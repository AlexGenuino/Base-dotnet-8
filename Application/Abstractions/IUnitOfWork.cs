namespace Application.Abstractions;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    ICardRepository CardRepository { get; }
    Task CommitAsync(CancellationToken cancellationToken = default);
}
