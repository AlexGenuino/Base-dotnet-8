using Application.UserCQ.Commands;
using FluentValidation;

namespace Application.UserCQ.Validators
{
    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("O campo 'Username'não pode estar vazio.");
        }
    }
}
