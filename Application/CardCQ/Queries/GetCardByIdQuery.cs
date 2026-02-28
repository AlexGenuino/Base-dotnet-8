using Application.CardCQ.ViewModels;
using MediatR;

namespace Application.CardCQ.Queries;

public record GetCardByIdQuery(Guid Id) : IRequest<CardViewModel?>;
