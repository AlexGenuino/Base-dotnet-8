using Application.Response;
using Application.UserCQ.ViewModels;
using MediatR;
using OneOf;

namespace Application.UserCQ.Commands
{
    public record RefreshTokenCommand : IRequest<OneOf<RefreshTokenViewModel, ErrorInfo>>
    {
        public string? Username { get; set; }
        public string? RefreshToken { get; set; }
    }
}
