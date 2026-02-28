using Application.CardCQ.ViewModels;
using Application.Response;
using MediatR;
using OneOf;

namespace Application.CardCQ.Commands;

public record UpdateCardCommand : IRequest<OneOf<CardViewModel, ErrorInfo>>
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? Deadline { get; set; }
    public Guid ListId { get; set; }
    public Domain.Enum.StatusCard Status { get; set; }
}
