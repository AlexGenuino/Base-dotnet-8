using Application.CardCQ.Commands;
using Application.CardCQ.Handlers;
using Application.CardCQ.ViewModels;
using Domain.Enum;

namespace Application.Tests.CardCQ;

public class UpdateCardCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICardRepository> _cardRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdateCardCommandHandler _handler;

    public UpdateCardCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _cardRepositoryMock = new Mock<ICardRepository>();
        _unitOfWorkMock.Setup(u => u.CardRepository).Returns(_cardRepositoryMock.Object);
        _mapperMock = new Mock<IMapper>();
        _handler = new UpdateCardCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_QuandoCardExiste_AtualizaERetornaCardViewModel()
    {
        var id = Guid.NewGuid();
        var listId = Guid.NewGuid();
        var card = new Card
        {
            Id = id,
            Title = "Antigo",
            Description = "Desc",
            ListId = listId,
            Status = StatusCard.Todo
        };
        var command = new UpdateCardCommand
        {
            Id = id,
            Title = "Atualizado",
            Description = "Nova desc",
            ListId = listId,
            Status = StatusCard.Done
        };
        var viewModel = new CardViewModel { Id = id, Title = command.Title, ListId = listId, Status = StatusCard.Done };

        _cardRepositoryMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(card);
        _cardRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Card>(), It.IsAny<CancellationToken>())).ReturnsAsync((Card c, CancellationToken _) => c);
        _mapperMock.Setup(m => m.Map<CardViewModel>(It.IsAny<Card>())).Returns(viewModel);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsT0);
        Assert.Equal("Atualizado", result.AsT0.Title);
        Assert.Equal(StatusCard.Done, result.AsT0.Status);
        _cardRepositoryMock.Verify(r => r.UpdateAsync(card, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoCardNaoExiste_RetornaErrorInfo404()
    {
        var id = Guid.NewGuid();
        var command = new UpdateCardCommand { Id = id, Title = "T", ListId = Guid.NewGuid(), Status = StatusCard.Todo };

        _cardRepositoryMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Card?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsT1);
        var error = result.AsT1;
        Assert.Equal(404, error.HttpStatus);
        Assert.Contains("não encontrado", error.ErrorDescription!);
        _cardRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Card>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
