using System.ComponentModel;

namespace Domain.Enum
{
    public enum BusinessError
    {
        [Description("Erro interno, por favor tente novamente mais tarde.")]
        InternalServerError,

        [Description("Nenhum usuário encontrado com o email informado.")]
        EmailNotFound,

        [Description("A senha informada está incorreta.")]
        PasswordNotFound,

        [Description("Refresh Token inválido ou expirado. Faça login novamente.")]
        InvalidRefreshToken,
    }
}
