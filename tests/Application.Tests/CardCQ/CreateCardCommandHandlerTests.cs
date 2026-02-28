using Application.CardCQ.Commands;
using Application.CardCQ.Handlers;
using Application.CardCQ.ViewModels;
using Domain.Enum;

namespace Application.Tests.CardCQ;

public class CreateCardCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICardRepository> _cardRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateCardCommandHandler _handler;

    public CreateCardCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _cardRepositoryMock = new Mock<ICardRepository>();
        _unitOfWorkMock.Setup(u => u.CardRepository).Returns(_cardRepositoryMock.Object);
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateCardCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_DeveCriarCard_RetornarCardViewModel()
    {
        var listId = Guid.NewGuid();
        var command = new CreateCardCommand
        {
            Title = "Novo Card",
            Description = "Descrição",
            ListId = listId,
            Status = StatusCard.Todo
        };
        var card = new Card
        {
            Id = Guid.NewGuid(),
            Title = command.Title,
            Description = command.Description,
            ListId = listId,
            Status = command.Status
        };
        var viewModel = new CardViewModel
        {
            Id = card.Id,
            Title = card.Title,
            Description = card.Description,
            ListId = listId,
            Status = card.Status
        };

        _mapperMock.Setup(m => m.Map<Card>(command)).Returns(card);
        _mapperMock.Setup(m => m.Map<CardViewModel>(It.IsAny<Card>())).Returns(viewModel);
        _cardRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Card>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Card c, CancellationToken _) => c);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsT0);
        var vm = result.AsT0;
        Assert.Equal(command.Title, vm.Title);
        Assert.Equal(listId, vm.ListId);
        _cardRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Card>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
