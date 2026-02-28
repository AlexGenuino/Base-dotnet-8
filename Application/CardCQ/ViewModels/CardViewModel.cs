using Domain.Enum;

namespace Application.CardCQ.ViewModels;

public record CardViewModel
{
    public Guid Id { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public DateTime? CreatedAt { get; init; }
    public DateTime? Deadline { get; init; }
    public Guid ListId { get; init; }
    public StatusCard Status { get; init; }
}
