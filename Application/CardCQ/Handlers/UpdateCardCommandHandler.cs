using Application.Abstractions;
using Application.CardCQ.Commands;
using Application.CardCQ.ViewModels;
using Application.Response;
using AutoMapper;
using MediatR;
using OneOf;

namespace Application.CardCQ.Handlers;

public class UpdateCardCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateCardCommand, OneOf<CardViewModel, ErrorInfo>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<OneOf<CardViewModel, ErrorInfo>> Handle(UpdateCardCommand request, CancellationToken cancellationToken)
    {
        var card = await _unitOfWork.CardRepository.GetByIdAsync(request.Id, cancellationToken);
        if (card is null)
            return new ErrorInfo("Card não encontrado.", 404);

        card.Title = request.Title;
        card.Description = request.Description;
        card.Deadline = request.Deadline;
        card.ListId = request.ListId;
        card.Status = request.Status;

        await _unitOfWork.CardRepository.UpdateAsync(card, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return _mapper.Map<CardViewModel>(card);
    }
}
