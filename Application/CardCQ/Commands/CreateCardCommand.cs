using Application.CardCQ.ViewModels;
using Application.Response;
using MediatR;
using OneOf;

namespace Application.CardCQ.Commands;

public record CreateCardCommand : IRequest<OneOf<CardViewModel, ErrorInfo>>
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? Deadline { get; set; }
    public Guid ListId { get; set; }
    public Domain.Enum.StatusCard Status { get; set; } = Domain.Enum.StatusCard.Todo;
}
