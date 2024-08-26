using APIArena.DTO;
using APIArena.Models;

namespace APIArena.Services
{
    public class MapService
    {
        public MapDTO InitializeMap()
        {
            
            MapDTO map = new(12, 12);
            for (int i = 0; i < map.Height; i++)
            {
                List<TileDTO> row = new();
                for (int j = 0; j < map.Width; j++)
                {
                    row.Add(new TileDTO { Type = TileDTO.TileType.Empty, X = j, Y = i });
                }
                map.Tiles.Add(row);
            }

            return map;
        }
    }
}
