using Application.UserCQ.Commands;
using Application.UserCQ.Handlers;
using Application.UserCQ.ViewModels;
using Domain.Abstractions;
using Microsoft.Extensions.Configuration;

namespace Application.Tests.UserCQ;

public class RefreshTokenCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock.Setup(u => u.UserRepository).Returns(_userRepositoryMock.Object);
        _authServiceMock = new Mock<IAuthService>();
        _configurationMock = new Mock<IConfiguration>();
        _configurationMock.Setup(c => c["JWT:RefreshTokenExpirationTimeInDays"]).Returns("7");
        _mapperMock = new Mock<IMapper>();
        _handler = new RefreshTokenCommandHandler(
            _unitOfWorkMock.Object,
            _authServiceMock.Object,
            _configurationMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_QuandoUsuarioNaoExiste_RetornaErrorInfoInvalidRefreshToken()
    {
        var command = new RefreshTokenCommand { Username = "user", RefreshToken = "token" };
        _userRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsT1);
        Assert.Contains("Refresh", result.AsT1.ErrorDescription!, StringComparison.OrdinalIgnoreCase);
        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_QuandoRefreshTokenNaoConfere_RetornaErrorInfoInvalidRefreshToken()
    {
        var command = new RefreshTokenCommand { Username = "user", RefreshToken = "token-errado" };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "user",
            Email = "user@mail.com",
            RefreshToken = "token-correto",
            RefreshTokenExpirationTime = DateTime.UtcNow.AddDays(1)
        };
        _userRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsT1);
        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_QuandoRefreshTokenExpirado_RetornaErrorInfoInvalidRefreshToken()
    {
        var command = new RefreshTokenCommand { Username = "user", RefreshToken = "token" };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "user",
            Email = "user@mail.com",
            RefreshToken = "token",
            RefreshTokenExpirationTime = DateTime.UtcNow.AddDays(-1)
        };
        _userRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsT1);
        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_QuandoRefreshTokenValido_AtualizaERetornaRefreshTokenViewModel()
    {
        var command = new RefreshTokenCommand { Username = "user", RefreshToken = "token-valido" };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "user",
            Email = "user@mail.com",
            RefreshToken = "token-valido",
            RefreshTokenExpirationTime = DateTime.UtcNow.AddDays(1)
        };
        var refreshVm = new RefreshTokenViewModel { Email = user.Email, Username = user.Username, TokenJWT = "novo-jwt", RefreshToken = "novo-refresh" };

        _userRepositoryMock
            .Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _authServiceMock.Setup(a => a.GenerateRefreshToken()).Returns("novo-refresh");
        _authServiceMock.Setup(a => a.GenerateJWT(user.Email!, user.Username!)).Returns("novo-jwt");
        _mapperMock.Setup(m => m.Map<RefreshTokenViewModel>(It.IsAny<User>())).Returns(refreshVm);
        _userRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ReturnsAsync((User u, CancellationToken _) => u);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsT0);
        Assert.Equal("novo-jwt", result.AsT0.TokenJWT);
        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
