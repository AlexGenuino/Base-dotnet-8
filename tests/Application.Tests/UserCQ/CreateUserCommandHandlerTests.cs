using Application.UserCQ.Commands;
using Application.UserCQ.Handlers;
using Application.UserCQ.ViewModels;
using Domain.Abstractions;
using Domain.Enum;

namespace Application.Tests.UserCQ;

public class CreateUserCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock.Setup(u => u.UserRepository).Returns(_userRepositoryMock.Object);
        _authServiceMock = new Mock<IAuthService>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateUserCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _authServiceMock.Object);
    }

    [Fact]
    public async Task Handle_QuandoEmailJaExiste_RetornaErrorInfoEmailUnavailable()
    {
        var command = new CreateUserCommand { Email = "existente@mail.com", Username = "novo", Password = "senha123" };
        _userRepositoryMock.Setup(r => r.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _userRepositoryMock.Setup(r => r.ExistsByUsernameAsync(command.Username!, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _authServiceMock
            .Setup(a => a.GetValidationErrorForEmailAndUsername(true, false))
            .Returns(ValidationFieldsUser.EmailUnavailable);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsT1);
        Assert.Contains("email", result.AsT1.ErrorDescription!, StringComparison.OrdinalIgnoreCase);
        _userRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_QuandoUsernameJaExiste_RetornaErrorInfoUsernameUnavailable()
    {
        var command = new CreateUserCommand { Email = "novo@mail.com", Username = "existente", Password = "senha123" };
        _userRepositoryMock.Setup(r => r.ExistsByEmailAsync(command.Email!, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _userRepositoryMock.Setup(r => r.ExistsByUsernameAsync(command.Username!, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _authServiceMock
            .Setup(a => a.GetValidationErrorForEmailAndUsername(false, true))
            .Returns(ValidationFieldsUser.UsernameUnavailable);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsT1);
        Assert.Contains("username", result.AsT1.ErrorDescription!, StringComparison.OrdinalIgnoreCase);
        _userRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_QuandoEmailEUsernameJaExistem_RetornaErrorInfoAmbosUnavailable()
    {
        var command = new CreateUserCommand { Email = "existente@mail.com", Username = "existente", Password = "senha123" };
        _userRepositoryMock.Setup(r => r.ExistsByEmailAsync(command.Email!, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _userRepositoryMock.Setup(r => r.ExistsByUsernameAsync(command.Username!, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _authServiceMock
            .Setup(a => a.GetValidationErrorForEmailAndUsername(true, true))
            .Returns(ValidationFieldsUser.UsernameAndEmailUnavailable);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsT1);
        _userRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_QuandoEmailEUsernameUnicos_CriaUsuarioERetornaRefreshTokenViewModel()
    {
        var command = new CreateUserCommand
        {
            Name = "Nome",
            Surname = "Sobrenome",
            Email = "novo@mail.com",
            Username = "novouser",
            Password = "senha123"
        };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Surname = command.Surname,
            Email = command.Email,
            Username = command.Username,
            PasswordHash = "hash"
        };
        var refreshVm = new RefreshTokenViewModel
        {
            Email = user.Email,
            Username = user.Username,
            TokenJWT = "jwt-token",
            RefreshToken = "refresh-token"
        };

        _userRepositoryMock.Setup(r => r.ExistsByEmailAsync(command.Email!, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _userRepositoryMock.Setup(r => r.ExistsByUsernameAsync(command.Username!, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _authServiceMock.Setup(a => a.GetValidationErrorForEmailAndUsername(false, false)).Returns((ValidationFieldsUser?)null);
        _authServiceMock.Setup(a => a.GenerateRefreshToken()).Returns("refresh-token");
        _authServiceMock.Setup(a => a.HashingPassword(command.Password!)).Returns("hash");
        _authServiceMock.Setup(a => a.GenerateJWT(user.Email!, user.Username!)).Returns("jwt-token");
        _mapperMock.Setup(m => m.Map<User>(command)).Returns(user);
        _mapperMock.Setup(m => m.Map<RefreshTokenViewModel>(It.IsAny<User>())).Returns(refreshVm);
        _userRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ReturnsAsync((User u, CancellationToken _) => u);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsT0);
        Assert.Equal("novo@mail.com", result.AsT0.Email);
        Assert.Equal("jwt-token", result.AsT0.TokenJWT);
        _userRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
