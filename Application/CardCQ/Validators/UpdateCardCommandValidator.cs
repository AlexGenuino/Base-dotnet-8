using Application.CardCQ.Commands;
using FluentValidation;

namespace Application.CardCQ.Validators;

public class UpdateCardCommandValidator : AbstractValidator<UpdateCardCommand>
{
    public UpdateCardCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("O Id do card é obrigatório.");
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("O título é obrigatório.")
            .MaximumLength(45).WithMessage("O título deve ter no máximo 45 caracteres.");
        RuleFor(x => x.Description)
            .MaximumLength(120).When(x => !string.IsNullOrEmpty(x.Description));
        RuleFor(x => x.ListId).NotEmpty().WithMessage("O ListId é obrigatório.");
    }
}
