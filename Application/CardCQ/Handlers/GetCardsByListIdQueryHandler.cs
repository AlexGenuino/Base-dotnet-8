using Application.Abstractions;
using Application.CardCQ.Queries;
using Application.CardCQ.ViewModels;
using AutoMapper;
using MediatR;

namespace Application.CardCQ.Handlers;

public class GetCardsByListIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetCardsByListIdQuery, IReadOnlyList<CardViewModel>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<IReadOnlyList<CardViewModel>> Handle(GetCardsByListIdQuery request, CancellationToken cancellationToken)
    {
        var cards = await _unitOfWork.CardRepository.GetByListIdAsync(request.ListId, cancellationToken);
        return _mapper.Map<IReadOnlyList<CardViewModel>>(cards);
    }
}
