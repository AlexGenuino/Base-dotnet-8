using Application.Abstractions;
using Application.CardCQ.Commands;
using Application.CardCQ.ViewModels;
using Application.Response;
using AutoMapper;
using Domain.Entity;
using MediatR;
using OneOf;

namespace Application.CardCQ.Handlers;

public class CreateCardCommandHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateCardCommand, OneOf<CardViewModel, ErrorInfo>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<OneOf<CardViewModel, ErrorInfo>> Handle(CreateCardCommand request, CancellationToken cancellationToken)
    {
        var card = _mapper.Map<Card>(request);
        card.Id = Guid.NewGuid();
        card.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.CardRepository.CreateAsync(card, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return _mapper.Map<CardViewModel>(card);
    }
}
