using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIArena.Models  
{
    public class Session
    {
        [Key]
        public required Guid Id { get; set; }
        public required Guid Player1Id { get; set; }
        public required Guid Player2Id { get; set; }
        public required Guid ArenaId { get; set; }
        [ForeignKey(nameof(Player1Id))]
        public virtual Player Player1 { get; set; } = default!;
        [ForeignKey(nameof(Player2Id))]
        public virtual Player Player2 { get; set; } = default!;
        [ForeignKey(nameof(ArenaId))]
        public virtual Arena Arena { get; set; } = default!;
    }
}
