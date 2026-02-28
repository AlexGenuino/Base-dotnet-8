using Application.UserCQ.Commands;
using Application.UserCQ.ViewModels;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// Classe que contém os métodos action da entidade User.
    /// </summary>
    [Route("auth")]
    [ApiController]
    public class AuthController(IMediator mediator, IConfiguration configuration, IMapper mapper, IHttpContextAccessor contextAccessor) : BaseController(contextAccessor, configuration)
    {
        private readonly IMediator _mediator = mediator;
        private readonly IMapper _mapper = mapper;

        [HttpPost("create-user")]
        public async Task<IResult> Create(CreateUserCommand command)
        {
            var result = await _mediator.Send(command);

            return result.Match( response => {
                GenerateAuthCookie(response);
                return Results.Ok(response);
            }, error => Results.BadRequest(error));
        }

        [HttpPost("login")]
        public async Task<IResult> Login(LoginUserCommand command)
        {
            var result = await _mediator.Send(command);

            return result.Match(response => {
                GenerateAuthCookie(response);
                return Results.Ok(_mapper.Map<UserInfoViewModel>(response));
            }, error => Results.BadRequest(error));
        }

        [HttpPost("refresh-token")]
        public async Task<IResult> RefreshToken(RefreshTokenCommand command)
        {

            var result = await _mediator.Send(new RefreshTokenCommand { Username = command.Username, RefreshToken = Request.Cookies["refreshToken"] });

            return result.Match(response => {
                GenerateAuthCookie(response);
                return Results.Ok(_mapper.Map<UserInfoViewModel>(response));
            }, error => Results.BadRequest(error));
        }
    }
}
