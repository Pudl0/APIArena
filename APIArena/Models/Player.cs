using System.ComponentModel.DataAnnotations;

namespace APIArena.Models
{
    public class Player
    {
        [Key]
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required int Level { get; set; }
        public required int Experience { get; set; }
        public required int Gold { get; set; }

    }
}
