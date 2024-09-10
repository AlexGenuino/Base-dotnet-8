using Application.UserCQ.Commands;
using FluentValidation;

namespace Application.UserCQ.Validators
{
    public class CreateUserComandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserComandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("O campo 'email'não pode ser vazio.")
                .EmailAddress().WithMessage("O campo 'email'não é válido.")
                .WithErrorCode("400");
            RuleFor(x => x.Username).NotEmpty().WithMessage("O campo 'username' não pode estar vazio.");
        }
    }
}
