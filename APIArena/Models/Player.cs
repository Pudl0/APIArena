using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIArena.Models
{
    public class Player
    {
        [Key]
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        [MaxLength(32)]
        public required byte[] ApiKeyId { get; set; }
        public bool PlayedTurn { get; set; } = false;
        public required int Level { get; set; }
        public required int Gold { get; set; }
        public required int XPos { get; set; }
        public required int YPos { get; set; }
        [ForeignKey(nameof(ApiKeyId))]
        public virtual ApiKey? ApiKey { get; set; } = default!;
    }
}
