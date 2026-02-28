using Application.CardCQ.Commands;
using Application.CardCQ.Queries;
using Application.CardCQ.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CardsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>Obtém um card pelo Id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CardViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCardByIdQuery(id), cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result);
    }

    /// <summary>Lista cards por ListId.</summary>
    [HttpGet("list/{listId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<CardViewModel>), StatusCodes.Status200OK)]
    public async Task<IResult> GetByListId(Guid listId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCardsByListIdQuery(listId), cancellationToken);
        return Results.Ok(result);
    }

    /// <summary>Cria um novo card.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CardViewModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IResult> Create([FromBody] CreateCardCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return result.Match(
            card => Results.Created($"/api/cards/{card.Id}", card),
            error => Results.BadRequest(error));
    }

    /// <summary>Atualiza um card existente.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CardViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> Update(Guid id, [FromBody] UpdateCardCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return Results.BadRequest(new Application.Response.ErrorInfo("Id da rota não confere com o Id do corpo."));

        var result = await _mediator.Send(command, cancellationToken);
        return result.Match(
            card => Results.Ok(card),
            error => error.HttpStatus == 404 ? Results.NotFound() : Results.BadRequest(error));
    }

    /// <summary>Remove um card.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteCardCommand(id), cancellationToken);
        return result.Match(
            _ => Results.NoContent(),
            error => error.HttpStatus == 404 ? Results.NotFound() : Results.BadRequest(error));
    }
}
