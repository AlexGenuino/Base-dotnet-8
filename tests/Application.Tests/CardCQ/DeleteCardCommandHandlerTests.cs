using Application.Abstractions;
using Application.CardCQ.Commands;
using Application.CardCQ.Handlers;
using Application.Response;
using Domain.Entity;
using Domain.Enum;
using MediatR;

namespace Application.Tests.CardCQ;

public class DeleteCardCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICardRepository> _cardRepositoryMock;
    private readonly DeleteCardCommandHandler _handler;

    public DeleteCardCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _cardRepositoryMock = new Mock<ICardRepository>();
        _unitOfWorkMock.Setup(u => u.CardRepository).Returns(_cardRepositoryMock.Object);
        _handler = new DeleteCardCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_QuandoCardExiste_DeletaERetornaUnit()
    {
        var id = Guid.NewGuid();
        var card = new Card { Id = id, Title = "Card", ListId = Guid.NewGuid(), Status = StatusCard.Todo };
        var command = new DeleteCardCommand(id);

        _cardRepositoryMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(card);
        _cardRepositoryMock.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsT0);
        Assert.Equal(Unit.Value, result.AsT0);
        _cardRepositoryMock.Verify(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_QuandoCardNaoExiste_RetornaErrorInfo404()
    {
        var id = Guid.NewGuid();
        var command = new DeleteCardCommand(id);

        _cardRepositoryMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Card?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsT1);
        var error = result.AsT1;
        Assert.Equal(404, error.HttpStatus);
        Assert.Contains("não encontrado", error.ErrorDescription!);
        _cardRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
