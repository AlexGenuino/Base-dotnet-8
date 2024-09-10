using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity
{
    public class User
    {
        [Key]
        [Required]
        public Guid Id { get; init; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        [Column(TypeName = "nvarchar(50)")]
        public string? Name { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Column(TypeName = "nvarchar(50)")]
        public string? Surname { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? PasswordHash { get; set; }

        [Required]
        public string? Username { get; set; }

        public ICollection<Workspace>? Workspaces { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpirationTime { get; set; }
    }
}
