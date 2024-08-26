using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using APIArena.DTO;
using Newtonsoft.Json;

namespace APIArena.Models
{
    public class Map
    {
        [Key]
        public required Guid Id { get; set; }
        public required int Height { get; set; }
        public required int Width { get; set; }
        public string SerializedTiles { get; set; } = default!;

        // Tiles get serialized to JSON when stored in the database and deserialized when retrieved cause EF Core doesn't support nested collections
        [NotMapped]
        public List<List<TileDTO>> Tiles
        {
            get => JsonConvert.DeserializeObject<List<List<TileDTO>>>(SerializedTiles) ?? new List<List<TileDTO>>();
            set => SerializedTiles = JsonConvert.SerializeObject(value);
        }

        [NotMapped]
        public MapDTO MapDTO => new(Height, Width)
        {
            Id = Id,
            Tiles = Tiles,
        };
        public static Map FromMapDTO(MapDTO mapDto)
        {
            return new Map()
            {
                Height = mapDto.Height,
                Width = mapDto.Width,
                Id = mapDto.Id,
                Tiles = mapDto.Tiles
            };
        }
    }
}
