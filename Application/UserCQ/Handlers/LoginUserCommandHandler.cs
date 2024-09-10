using Application.Response;
using Application.UserCQ.Commands;
using Application.UserCQ.ViewModels;
using AutoMapper;
using Domain.Abstractions;
using Domain.Enum;
using Domain.Utils;
using Infra.Persistence;
using Infra.Repository.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Configuration;
using OneOf;

namespace Application.UserCQ.Handlers
{
    public class LoginUserCommandHandler(IConfiguration configuration, IUnitOfWork unitOfWork, IAuthService authService, IMapper mapper) : IRequestHandler<LoginUserCommand, OneOf<RefreshTokenViewModel, ErrorInfo>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IAuthService _authService = authService;
        private readonly IConfiguration _configuration = configuration;
        private readonly IMapper _mapper = mapper;

        public async Task<OneOf<RefreshTokenViewModel, ErrorInfo>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.Get(x => x.Email == request.Email);

            if (user is null)
                return new ErrorInfo(BusinessError.EmailNotFound.GetDescription());

            var hashPasswordRequest = _authService.HashingPassword(request.Password!);

            if (hashPasswordRequest != user.PasswordHash)
                return new ErrorInfo(BusinessError.PasswordNotFound.GetDescription());

            _ = int.TryParse(_configuration["JWT:RefreshTokenExpirationTimeInDays"], out int refreshTokenValidityInDays);

            user.RefreshToken = _authService.GenerateRefreshToken();
            user.RefreshTokenExpirationTime = DateTime.Now.AddDays(refreshTokenValidityInDays);

            await _unitOfWork.UserRepository.Update(user);
            _unitOfWork.Commit();

            RefreshTokenViewModel refreshTokenVM = _mapper.Map<RefreshTokenViewModel>(user);
            refreshTokenVM.TokenJWT = _authService.GenerateJWT(user.Email!, user.Username!);

            return refreshTokenVM;
        }
    }
}
