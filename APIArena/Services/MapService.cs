using APIArena.DTO;
using APIArena.Models;
using APIArena.Server;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

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
            map = GenerateRessourcesAsync(map);

            return map;
        }
        private static MapDTO GenerateRessourcesAsync(MapDTO mapDto)
        {
            const int spawnrate = 4;
            Random random = new();
            
            int ressourceCount = mapDto.Width * mapDto.Height / spawnrate; // Replace with the number of ones you want to place

            for (int i = 0; i < ressourceCount; i++)
            {
                int x, y;
                do
                {
                    x = random.Next(0, mapDto.Width);
                    y = random.Next(0, mapDto.Height);
                }
                // Ensure that the selected coordinates are not [0][0], [max-1][max-1], or already set to 1
                while ((x == 0 && y == 0) || (x == mapDto.Width - 1 && y == mapDto.Height - 1) || mapDto.Tiles[x][y].Type == TileDTO.TileType.Gold);
                mapDto.Tiles[x][y].Type = TileDTO.TileType.Gold;
            }

            return mapDto;
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
            var existingMap = await _context.Maps.FindAsync(mapDto.Id);
            if (existingMap != null)
            {
                _context.Entry(existingMap).CurrentValues.SetValues(mapDto);
                await _context.SaveChangesAsync();
            }
            else
            {
                var newMap = Map.FromMapDTO(mapDto);
                _context.Maps.Add(newMap);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<MapDTO?> GetMapDTOByIdAsync(Guid id)
        {
            Map? map = await _context.Maps.FirstOrDefaultAsync(m => m.Id == id);
            if (map == null)
                return null;

            return map.MapDTO;
        }
        public async Task<bool> DeleteMapAsync(Guid id)
        {

            Map? map = await _context.Maps.FirstOrDefaultAsync(m => m.Id == id);
            if (map == null)
                return false;

            _context.Maps.Remove(map);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
