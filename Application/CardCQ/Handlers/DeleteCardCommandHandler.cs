using Application.Abstractions;
using Application.CardCQ.Commands;
using Application.Response;
using MediatR;
using OneOf;

namespace Application.CardCQ.Handlers;

public class DeleteCardCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteCardCommand, OneOf<Unit, ErrorInfo>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<OneOf<Unit, ErrorInfo>> Handle(DeleteCardCommand request, CancellationToken cancellationToken)
    {
        var card = await _unitOfWork.CardRepository.GetByIdAsync(request.Id, cancellationToken);
        if (card is null)
            return new ErrorInfo("Card não encontrado.", 404);

        await _unitOfWork.CardRepository.DeleteAsync(request.Id, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Unit.Value;
    }
}
