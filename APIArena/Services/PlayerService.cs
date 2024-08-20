using APIArena.Models;
using APIArena.Server;

namespace APIArena.Services
{
    public class PlayerService(DataContext _context)
    {
        public async Task<Guid> CreatePlayerAsync(string name)
        {
            Guid id = Guid.NewGuid();
            Player player = new()
            {
                Id = id,
                Name = name,
                Level = 1,
                Experience = 0,
                Gold = 0
            };

            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            return id;
        }
    }
}
