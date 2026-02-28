using Application.CardCQ.Handlers;
using Application.CardCQ.Queries;
using Application.CardCQ.ViewModels;
using Domain.Enum;

namespace Application.Tests.CardCQ;

public class GetCardsByListIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICardRepository> _cardRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetCardsByListIdQueryHandler _handler;

    public GetCardsByListIdQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _cardRepositoryMock = new Mock<ICardRepository>();
        _unitOfWorkMock.Setup(u => u.CardRepository).Returns(_cardRepositoryMock.Object);
        _mapperMock = new Mock<IMapper>();
        _handler = new GetCardsByListIdQueryHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_RetornaListaDeCardViewModels()
    {
        var listId = Guid.NewGuid();
        var cards = new List<Card>
        {
            new() { Id = Guid.NewGuid(), Title = "C1", ListId = listId, Status = StatusCard.Todo },
            new() { Id = Guid.NewGuid(), Title = "C2", ListId = listId, Status = StatusCard.Done }
        };
        var viewModels = new List<CardViewModel>
        {
            new() { Id = cards[0].Id, Title = "C1", ListId = listId, Status = StatusCard.Todo },
            new() { Id = cards[1].Id, Title = "C2", ListId = listId, Status = StatusCard.Done }
        };
        var query = new GetCardsByListIdQuery(listId);

        _cardRepositoryMock.Setup(r => r.GetByListIdAsync(listId, It.IsAny<CancellationToken>())).ReturnsAsync(cards);
        _mapperMock.Setup(m => m.Map<IReadOnlyList<CardViewModel>>(cards)).Returns(viewModels);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("C1", result[0].Title);
        Assert.Equal("C2", result[1].Title);
        _cardRepositoryMock.Verify(r => r.GetByListIdAsync(listId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoListaVazia_RetornaListaVazia()
    {
        var listId = Guid.NewGuid();
        var query = new GetCardsByListIdQuery(listId);

        _cardRepositoryMock.Setup(r => r.GetByListIdAsync(listId, It.IsAny<CancellationToken>())).ReturnsAsync(new List<Card>());
        _mapperMock.Setup(m => m.Map<IReadOnlyList<CardViewModel>>(It.IsAny<IReadOnlyList<Card>>())).Returns(new List<CardViewModel>());

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
