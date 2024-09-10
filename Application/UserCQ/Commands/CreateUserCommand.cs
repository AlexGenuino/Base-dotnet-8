using Application.Response;
using Application.UserCQ.ViewModels;
using MediatR;
using OneOf;

namespace Application.UserCQ.Commands
{
    public record CreateUserCommand : IRequest<OneOf<RefreshTokenViewModel, ErrorInfo>>
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Username { get; set; }
    }
}
