using Application.Abstractions;
using Application.Response;
using Application.UserCQ.Commands;
using Application.UserCQ.ViewModels;
using AutoMapper;
using Domain.Abstractions;
using Domain.Entity;
using Domain.Utils;
using MediatR;
using OneOf;

namespace Application.UserCQ.Handlers
{
    public class CreateUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IAuthService authService) : IRequestHandler<CreateUserCommand, OneOf<RefreshTokenViewModel, ErrorInfo>>
    {
        private readonly IAuthService _authService = authService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<OneOf<RefreshTokenViewModel, ErrorInfo>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var emailExists = await _unitOfWork.UserRepository.ExistsByEmailAsync(request.Email!, cancellationToken);
            var usernameExists = await _unitOfWork.UserRepository.ExistsByUsernameAsync(request.Username!, cancellationToken);
            var validationError = _authService.GetValidationErrorForEmailAndUsername(emailExists, usernameExists);

            if (validationError is not null)
                return new ErrorInfo(validationError.Value.GetDescription());

            var user = _mapper.Map<User>(request);
            user.RefreshToken = _authService.GenerateRefreshToken();
            user.PasswordHash = _authService.HashingPassword(request.Password!);

            await _unitOfWork.UserRepository.CreateAsync(user, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            var refreshTokenVM = _mapper.Map<RefreshTokenViewModel>(user);
            refreshTokenVM.TokenJWT = _authService.GenerateJWT(user.Email!, user.Username!);

            return refreshTokenVM;
        }
    }
}
