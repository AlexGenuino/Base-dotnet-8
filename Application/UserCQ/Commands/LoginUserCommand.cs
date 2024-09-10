using Application.Response;
using Application.UserCQ.ViewModels;
using MediatR;
using OneOf;

namespace Application.UserCQ.Commands
{
    public record LoginUserCommand : IRequest<OneOf<RefreshTokenViewModel, ErrorInfo>>
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
