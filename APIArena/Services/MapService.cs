using APIArena.DTO;
using APIArena.Models;
using APIArena.Server;
using Microsoft.EntityFrameworkCore;

namespace APIArena.Services
{
    public class MapService(DataContext _context)
    {
        public async Task<MapDTO> InitializeMapAsync()
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

            map.Id = await CreateMapAsync(map);

            return map;
        }
        public async Task<Guid> CreateMapAsync(MapDTO mapDto)
        {
            Map map = Map.FromMapDTO(mapDto);
            _context.Maps.Add(map);
            await _context.SaveChangesAsync();
            return map.Id;
        }

        public async Task UpdateMapAsync(MapDTO mapDto)
        {
            Map map = Map.FromMapDTO(mapDto);
            _context.Maps.Update(map);
            await _context.SaveChangesAsync();
        }
        public async Task<MapDTO?> GetMapDTOByIdAsync(Guid id)
        {
            Map? map = await _context.Maps.FirstOrDefaultAsync(m => m.Id == id);
            if (map == null)
                return null;

            return map.MapDTO;
        }
    }
}
