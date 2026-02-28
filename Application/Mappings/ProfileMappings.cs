using Application.CardCQ.Commands;
using Application.CardCQ.ViewModels;
using Application.UserCQ.Commands;
using Application.UserCQ.ViewModels;
using AutoMapper;
using Domain.Entity;

namespace Application.Mappings
{
    public class ProfileMappings : Profile
    {
        public ProfileMappings()
        {
            CreateMap<CreateCardCommand, Card>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore());
            CreateMap<Card, CardViewModel>();

            CreateMap<CreateUserCommand, User>()
                .ForMember(d => d.Id, o => o.MapFrom(_ => Guid.NewGuid()))
                .ForMember(d => d.RefreshToken, o => o.Ignore())
                .ForMember(d => d.RefreshTokenExpirationTime, o => o.Ignore())
                .ForMember(d => d.PasswordHash, o => o.Ignore());

            CreateMap<User, RefreshTokenViewModel>()
                .ForMember(x => x.TokenJWT, x => x.AllowNull());


            CreateMap<RefreshTokenViewModel, UserInfoViewModel>();
        }
    }
}
