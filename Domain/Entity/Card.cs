using Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entity
{
    public class Card
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(45)]
        public string? Title { get; set; }

        [StringLength(120)]
        public string? Description { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public DateTime? Deadline { get; set; }

        public Guid ListId { get; set; }
        public ListCard? List { get; set; }

        public StatusCard Status { get; set; }
    }
}
