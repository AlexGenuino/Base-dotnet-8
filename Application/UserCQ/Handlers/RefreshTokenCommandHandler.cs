using Application.Response;
using Application.UserCQ.Commands;
using Application.UserCQ.ViewModels;
using Domain.Abstractions;
using Domain.Enum;
using Domain.Utils;
using Microsoft.Extensions.Configuration;

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
            var user = await _unitOfWork.UserRepository.GetAsync(x => x.Username == request.Username, cancellationToken);

            if (user is null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpirationTime < DateTime.UtcNow)
                return new ErrorInfo(BusinessError.InvalidRefreshToken.GetDescription());

            user.RefreshToken = _authService.GenerateRefreshToken();
            _ = int.TryParse(_configuration["JWT:RefreshTokenExpirationTimeInDays"], out int refreshTokenExpirationTimeInDays);
            user.RefreshTokenExpirationTime = DateTime.UtcNow.AddDays(refreshTokenExpirationTimeInDays);

            await _unitOfWork.UserRepository.UpdateAsync(user, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            var refreshTokenVM = _mapper.Map<RefreshTokenViewModel>(user);
            refreshTokenVM.TokenJWT = _authService.GenerateJWT(user.Email!, user.Username!);

            return refreshTokenVM;
        }
    }
}
