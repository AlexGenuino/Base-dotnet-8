using Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entity
{
    public class ListCard
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(45, MinimumLength = 2)]
        public string? Title { get; set; }

        public StatusItem Status { get; set; } = StatusItem.Active;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Workspace? Workspace { get; set; }

        public ICollection<Card>? Cards { get; set; }
    }
}
