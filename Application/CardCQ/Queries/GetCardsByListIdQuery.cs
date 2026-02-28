using Application.CardCQ.ViewModels;
using MediatR;

namespace Application.CardCQ.Queries;

public record GetCardsByListIdQuery(Guid ListId) : IRequest<IReadOnlyList<CardViewModel>>;
