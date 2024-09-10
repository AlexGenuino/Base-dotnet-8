using Application.Response;
using Application.UserCQ.Commands;
using Application.UserCQ.ViewModels;
using AutoMapper;
using Domain.Abstractions;
using Domain.Enum;
using Domain.Utils;
using Infra.Repository.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Configuration;
using OneOf;

namespace Application.UserCQ.Handlers
{
    public class RefreshTokenCommandHandler(IUnitOfWork unitOfWork, IAuthService authService, IConfiguration configuration, IMapper mapper) : IRequestHandler<RefreshTokenCommand, OneOf<RefreshTokenViewModel, ErrorInfo>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IAuthService _authService = authService;
        private readonly IConfiguration _configuration = configuration;
        private readonly IMapper _mapper = mapper;

        public async Task<OneOf<RefreshTokenViewModel, ErrorInfo>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.Get(x => x.Username == request.Username);

            if (user is null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpirationTime < DateTime.Now)
                return new ErrorInfo(BusinessError.InvalidRefreshToken.GetDescription());

            user.RefreshToken = _authService.GenerateRefreshToken();
            _ = int.TryParse(_configuration["JWT:RefreshTokenExpirationTimeInDays"], out int refreshTokenExpirationTimeInDays);
            user.RefreshTokenExpirationTime = DateTime.Now.AddDays(refreshTokenExpirationTimeInDays);

            _unitOfWork.Commit();

            var refreshTokenVM = _mapper.Map<RefreshTokenViewModel>(user);
            refreshTokenVM.TokenJWT = _authService.GenerateJWT(user.Email!, user.Username!);

            return refreshTokenVM;
        }
    }
}
