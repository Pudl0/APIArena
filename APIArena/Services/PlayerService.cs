using APIArena.DTO;
using APIArena.Models;
using APIArena.Server;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace APIArena.Services
{
    public class PlayerService(DataContext _context, MapService _mapService)
    {
        public async Task<Player> CreatePlayerAsync(string name, ApiKey apiKey)
        {
            Guid id = Guid.NewGuid();
            Player player = new()
            {
                Id = id,
                Name = name,
                Level = 1,
                Gold = 0,
                XPos = 0,
                YPos = 0,
                ApiKeyId = apiKey.Id
            };

            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            return player;
        }
        public async Task<Player?> GetPlayerByIdAsync(Guid id)
        => await _context.Players.FindAsync(id);

        public async Task<bool> UpdatePlayerAsync(Player player)
        {
            _context.Players.Update(player);
            return 0 < await _context.SaveChangesAsync();
        }

        public async Task<Player> GetPlayerByApiKeyAndSessionAsync(ApiKey apiKey, Session session)
        {
            List<Player> players = await _context.Players.Where(p => p.ApiKeyId == apiKey.Id).ToListAsync();
            return players.Where(players => players.Id == session.Player1Id || players.Id == session.Player2Id).First();
        }
        public async Task<Player?> MoveTop(Player player, MapDTO map)
        {
            player.YPos++;

            if (!map.ValidPlayerPosition(player))
                return null;

            await UpdatePlayerAsync(player);
            return player;
        }

        public async Task<Player?> MoveBottom(Player player, MapDTO map)
        {
            player.YPos--;

            if (!map.ValidPlayerPosition(player))
                return null;

            await UpdatePlayerAsync(player);
            return player;
        }

        public async Task<Player?> MoveLeft(Player player, MapDTO map)
        {
            player.XPos++;

            if (!map.ValidPlayerPosition(player))
                return null;

            await UpdatePlayerAsync(player);
            return player;
        }

        public async Task<Player?> MoveRight(Player player, MapDTO map)
        {
            player.XPos--;

            if (!map.ValidPlayerPosition(player))
                return null;

            await UpdatePlayerAsync(player);
            return player;
        }
    }
}
