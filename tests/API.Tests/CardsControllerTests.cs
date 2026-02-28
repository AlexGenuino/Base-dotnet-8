using API.Controllers;
using Application.CardCQ.Commands;
using Application.CardCQ.Queries;
using Application.CardCQ.ViewModels;
using Application.Response;
using Domain.Enum;
using Microsoft.Extensions.DependencyInjection;

namespace API.Tests;

public class CardsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly CardsController _controller;

    public CardsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new CardsController(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetById_QuandoCardExiste_Retorna200ComCard()
    {
        var id = Guid.NewGuid();
        var listId = Guid.NewGuid();
        var cardVm = new CardViewModel
        {
            Id = id,
            Title = "Card Teste",
            Description = "Desc",
            ListId = listId,
            Status = StatusCard.Todo
        };
        _mediatorMock
            .Setup(m => m.Send(It.Is<GetCardByIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cardVm);

        var result = await _controller.GetById(id, CancellationToken.None);

        await AssertResultStatusCode(result, 200);
    }

    [Fact]
    public async Task GetById_QuandoCardNaoExiste_Retorna404()
    {
        var id = Guid.NewGuid();
        _mediatorMock
            .Setup(m => m.Send(It.Is<GetCardByIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CardViewModel?)null);

        var result = await _controller.GetById(id, CancellationToken.None);

        await AssertResultStatusCode(result, 404);
    }

    [Fact]
    public async Task GetByListId_Retorna200ComListaDeCards()
    {
        var listId = Guid.NewGuid();
        var cards = new List<CardViewModel>
        {
            new() { Id = Guid.NewGuid(), Title = "C1", ListId = listId, Status = StatusCard.Todo }
        };
        _mediatorMock
            .Setup(m => m.Send(It.Is<GetCardsByListIdQuery>(q => q.ListId == listId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cards);

        var result = await _controller.GetByListId(listId, CancellationToken.None);

        await AssertResultStatusCode(result, 200);
    }

    [Fact]
    public async Task Create_QuandoSucesso_Retorna201()
    {
        var listId = Guid.NewGuid();
        var cmd = new CreateCardCommand
        {
            Title = "Novo Card",
            Description = "Desc",
            ListId = listId,
            Status = StatusCard.Todo
        };
        var created = new CardViewModel
        {
            Id = Guid.NewGuid(),
            Title = cmd.Title,
            Description = cmd.Description,
            ListId = listId,
            Status = StatusCard.Todo
        };
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateCardCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);

        var result = await _controller.Create(cmd, CancellationToken.None);

        await AssertResultStatusCode(result, 201);
    }

    [Fact]
    public async Task Create_QuandoFalha_Retorna400()
    {
        var cmd = new CreateCardCommand { Title = "", ListId = Guid.NewGuid() };
        var error = new ErrorInfo("Erro de validação");
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateCardCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(error);

        var result = await _controller.Create(cmd, CancellationToken.None);

        await AssertResultStatusCode(result, 400);
    }

    [Fact]
    public async Task Update_QuandoSucesso_Retorna200()
    {
        var id = Guid.NewGuid();
        var listId = Guid.NewGuid();
        var cmd = new UpdateCardCommand
        {
            Id = id,
            Title = "Atualizado",
            Description = "Desc",
            ListId = listId,
            Status = StatusCard.Done
        };
        var updated = new CardViewModel
        {
            Id = id,
            Title = cmd.Title,
            ListId = listId,
            Status = StatusCard.Done
        };
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateCardCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updated);

        var result = await _controller.Update(id, cmd, CancellationToken.None);

        await AssertResultStatusCode(result, 200);
    }

    [Fact]
    public async Task Update_QuandoIdDiferente_Retorna400()
    {
        var id = Guid.NewGuid();
        var cmd = new UpdateCardCommand { Id = Guid.NewGuid(), Title = "T", ListId = Guid.NewGuid(), Status = StatusCard.Todo };

        var result = await _controller.Update(id, cmd, CancellationToken.None);

        await AssertResultStatusCode(result, 400);
    }

    [Fact]
    public async Task Update_QuandoCardNaoExiste_Retorna404()
    {
        var id = Guid.NewGuid();
        var cmd = new UpdateCardCommand { Id = id, Title = "T", ListId = Guid.NewGuid(), Status = StatusCard.Todo };
        var error = new ErrorInfo("Card não encontrado.", 404);
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateCardCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(error);

        var result = await _controller.Update(id, cmd, CancellationToken.None);

        await AssertResultStatusCode(result, 404);
    }

    [Fact]
    public async Task Delete_QuandoSucesso_Retorna204()
    {
        var id = Guid.NewGuid();
        _mediatorMock
            .Setup(m => m.Send(It.Is<DeleteCardCommand>(c => c.Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var result = await _controller.Delete(id, CancellationToken.None);

        await AssertResultStatusCode(result, 204);
    }

    [Fact]
    public async Task Delete_QuandoCardNaoExiste_Retorna404()
    {
        var id = Guid.NewGuid();
        var error = new ErrorInfo("Card não encontrado.", 404);
        _mediatorMock
            .Setup(m => m.Send(It.Is<DeleteCardCommand>(c => c.Id == id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(error);

        var result = await _controller.Delete(id, CancellationToken.None);

        await AssertResultStatusCode(result, 404);
    }

    private static async Task AssertResultStatusCode(IResult result, int expectedStatusCode)
    {
        var context = new DefaultHttpContext();
        context.RequestServices = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();
        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;
        await result.ExecuteAsync(context);
        responseStream.Position = 0;
        Assert.Equal(expectedStatusCode, context.Response.StatusCode);
    }
}
