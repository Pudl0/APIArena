using APIArena.DTO;
using APIArena.Models;
using APIArena.Server;

namespace APIArena.Services
{
    public class PlayerService(DataContext _context)
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
    }
}
