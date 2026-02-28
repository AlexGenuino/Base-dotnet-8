using Application.UserCQ.Commands;
using Application.UserCQ.Handlers;
using Application.UserCQ.ViewModels;
using Domain.Abstractions;
using Microsoft.Extensions.Configuration;

namespace Application.Tests.UserCQ;

public class LoginUserCommandHandlerTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly LoginUserCommandHandler _handler;

    public LoginUserCommandHandlerTests()
    {
        _configurationMock = new Mock<IConfiguration>();
        _configurationMock.Setup(c => c["JWT:RefreshTokenExpirationTimeInDays"]).Returns("7");
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock.Setup(u => u.UserRepository).Returns(_userRepositoryMock.Object);
        _authServiceMock = new Mock<IAuthService>();
        _mapperMock = new Mock<IMapper>();
        _handler = new LoginUserCommandHandler(
            _configurationMock.Object,
            _unitOfWorkMock.Object,
            _authServiceMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_QuandoUsuarioNaoExiste_RetornaErrorInfoEmailNotFound()
    {
        var command = new LoginUserCommand { Email = "naoexiste@mail.com", Password = "senha" };
        _userRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsT1);
        Assert.Contains("email", result.AsT1.ErrorDescription!, StringComparison.OrdinalIgnoreCase);
        _authServiceMock.Verify(a => a.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_QuandoSenhaIncorreta_RetornaErrorInfoPasswordNotFound()
    {
        var command = new LoginUserCommand { Email = "user@mail.com", Password = "senhaerrada" };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = command.Email,
            Username = "user",
            PasswordHash = "hashcorreto"
        };
        _userRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _authServiceMock.Setup(a => a.VerifyPassword(command.Password!, user.PasswordHash!)).Returns(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsT1);
        Assert.Contains("senha", result.AsT1.ErrorDescription!, StringComparison.OrdinalIgnoreCase);
        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_QuandoCredenciaisCorretas_AtualizaRefreshTokenERetornaRefreshTokenViewModel()
    {
        var command = new LoginUserCommand { Email = "user@mail.com", Password = "senha123" };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = command.Email,
            Username = "user",
            PasswordHash = "hash"
        };
        var refreshVm = new RefreshTokenViewModel { Email = user.Email, Username = user.Username, TokenJWT = "jwt", RefreshToken = "refresh" };

        _userRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _authServiceMock.Setup(a => a.VerifyPassword(command.Password!, user.PasswordHash!)).Returns(true);
        _authServiceMock.Setup(a => a.GenerateRefreshToken()).Returns("refresh");
        _authServiceMock.Setup(a => a.GenerateJWT(user.Email!, user.Username!)).Returns("jwt");
        _mapperMock.Setup(m => m.Map<RefreshTokenViewModel>(It.IsAny<User>())).Returns(refreshVm);
        _userRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ReturnsAsync((User u, CancellationToken _) => u);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsT0);
        Assert.Equal("user@mail.com", result.AsT0.Email);
        Assert.Equal("jwt", result.AsT0.TokenJWT);
        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
