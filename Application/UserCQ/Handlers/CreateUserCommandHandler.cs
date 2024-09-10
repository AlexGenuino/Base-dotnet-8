using Application.Response;
using Application.UserCQ.Commands;
using Application.UserCQ.ViewModels;
using AutoMapper;
using Domain.Abstractions;
using MediatR;
using Domain.Entity;
using Infra.Repository.UnitOfWork;
using OneOf;
using Domain.Utils;

namespace Application.UserCQ.Handlers
{
    public class CreateUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IAuthService authService) : IRequestHandler<CreateUserCommand, OneOf<RefreshTokenViewModel, ErrorInfo>>
    {
        private readonly IAuthService _authService = authService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<OneOf<RefreshTokenViewModel, ErrorInfo>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var isUniqueEmailAndUsername = _authService.UniqueEmailAndUsername(request.Email!, request.Username!);

            if (isUniqueEmailAndUsername != null)
                return new ErrorInfo(isUniqueEmailAndUsername.GetDescription());

            var user = _mapper.Map<User>(request);
            user.RefreshToken = _authService.GenerateRefreshToken();
            user.PasswordHash = _authService.HashingPassword(request.Password!);

            await _unitOfWork.UserRepository.Create(user);
            _unitOfWork.Commit();

            var refreshTokenVM = _mapper.Map<RefreshTokenViewModel>(user);
            refreshTokenVM.TokenJWT = _authService.GenerateJWT(user.Email!, user.Username!);


            return refreshTokenVM;
        }
        
    }
}
