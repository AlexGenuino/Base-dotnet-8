using System.ComponentModel;

namespace Domain.Enum
{
    public enum ValidationFieldsUser
    {
        [Description("O username apresentado já está sendo utilizado. Tente outro.")]
        UsernameUnavailable,

        [Description("O Email apresentado já está sendo utilizado. Tente outro.")]
        EmailUnavailable,

        [Description("O username e o email apresentados já estão sendo utilizados. Tente outros.")]
        UsernameAndEmailUnavailable,
    }
}
