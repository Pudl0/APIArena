using System.ComponentModel.DataAnnotations;

namespace APIArena.Models
{
    public class Arena
    {
        [Key]
        public required Guid Id { get; set; }
        public required int Width { get; set; }
        public required int Height { get; set; }
    }
}
