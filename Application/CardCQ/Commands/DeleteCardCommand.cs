using Application.Response;
using MediatR;
using OneOf;

namespace Application.CardCQ.Commands;

public record DeleteCardCommand(Guid Id) : IRequest<OneOf<Unit, ErrorInfo>>;
