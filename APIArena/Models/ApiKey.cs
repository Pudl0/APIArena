using System.ComponentModel.DataAnnotations;

namespace APIArena.Models
{
    public class ApiKey
    {
        [Key]
        [MaxLength(32)]
        public required byte[] Id { get; set; }

        [MaxLength(16)]
        public required byte[] Salt { get; set; }

        [MaxLength(32)]
        public required string Name { get; set; }
        public required string Scopes { get; set; }
    }
}
