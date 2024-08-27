using APIArena.DTO;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIArena.Models
{
    public class Setting
    {
        [Key]
        public required Guid Id { get; set; }
        public required string Key { get; set; }
        public required string Value { get; set; }
        public required string Type { get; set; }
        
    }
}
