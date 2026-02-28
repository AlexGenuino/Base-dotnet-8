using Application.CardCQ.Handlers;
using Application.CardCQ.Queries;
using Application.CardCQ.ViewModels;
using Domain.Enum;

namespace Application.Tests.CardCQ;

public class GetCardByIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICardRepository> _cardRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetCardByIdQueryHandler _handler;

    public GetCardByIdQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _cardRepositoryMock = new Mock<ICardRepository>();
        _unitOfWorkMock.Setup(u => u.CardRepository).Returns(_cardRepositoryMock.Object);
        _mapperMock = new Mock<IMapper>();
        _handler = new GetCardByIdQueryHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_QuandoCardExiste_RetornaCardViewModel()
    {
        var id = Guid.NewGuid();
        var listId = Guid.NewGuid();
        var card = new Card { Id = id, Title = "Card", ListId = listId, Status = StatusCard.Todo };
        var viewModel = new CardViewModel { Id = id, Title = card.Title, ListId = listId, Status = StatusCard.Todo };
        var query = new GetCardByIdQuery(id);

        _cardRepositoryMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(card);
        _mapperMock.Setup(m => m.Map<CardViewModel>(card)).Returns(viewModel);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(id, result!.Id);
        Assert.Equal("Card", result.Title);
        _cardRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoCardNaoExiste_RetornaNull()
    {
        var id = Guid.NewGuid();
        var query = new GetCardByIdQuery(id);

        _cardRepositoryMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Card?)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
        _mapperMock.Verify(m => m.Map<CardViewModel>(It.IsAny<Card>()), Times.Never);
    }
}
