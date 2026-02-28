using Application.Abstractions;
using Application.CardCQ.Queries;
using Application.CardCQ.ViewModels;
using AutoMapper;
using MediatR;

namespace Application.CardCQ.Handlers;

public class GetCardByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetCardByIdQuery, CardViewModel?>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<CardViewModel?> Handle(GetCardByIdQuery request, CancellationToken cancellationToken)
    {
        var card = await _unitOfWork.CardRepository.GetByIdAsync(request.Id, cancellationToken);
        return card is null ? null : _mapper.Map<CardViewModel>(card);
    }
}
