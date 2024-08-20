using APIArena.DTO;

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

            AddPlayers(map);

            return map;
        }
        private MapDTO AddPlayers(MapDTO map)
        {
            map.Tiles[0][0].Type = TileDTO.TileType.Base;
            map.Tiles[map.Height - 1][map.Width - 1].Type = TileDTO.TileType.EnemyBase;

            return map;
        }
    }
}
