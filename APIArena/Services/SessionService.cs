using APIArena.DTO;
using APIArena.Models;
using APIArena.Server;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace APIArena.Services
{
    public class SessionService(DataContext _context, PlayerService _playerService)
    {
        public async Task<GameDTO> JoinOrCreateSessionAsync(Player player, MapDTO map)
        {
            List<Session> openSession = await _context.Sessions
                .Where(s => s.Player2Id == null).ToListAsync();

            if (openSession.Count > 0)
                return await JoinSessionAsync(openSession.First().Id, player, map);

            return await CreateSessionAsync(player.Id, map.Id);
        }
        private async Task<GameDTO> JoinSessionAsync(Guid id, Player player, MapDTO map)
        {
            Session? session = await _context.Sessions.FindAsync(id);
            session!.Player2Id = player.Id;
            await _context.SaveChangesAsync();

            player.XPos = map.Width - 1;
            player.YPos = map.Height - 1;
            await _playerService.UpdatePlayerAsync(player);

            return new GameDTO
            {
                Id = session.Id,
                Player = new PlayerDTO
                {
                    Id = player.Id,
                    X = player.XPos,
                    Y = player.YPos
                }
            };
        }
        private async Task<GameDTO> CreateSessionAsync(Guid playerId, Guid mapId)
        {
            Guid sessionId = Guid.NewGuid();

            Session? session = new()
            {
                Id = sessionId,
                Player1Id = playerId,
                MapId = mapId,
                Round = 0
            };

            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();

            await WaitingForSecondPlayer(sessionId);

            session = await GetSessionByIdAsync(sessionId);

            if (session!.Player2Id == null)
            {
                throw new Exception("Session Error");
            }

            return new GameDTO
            {
                Id = session.Id,
                Player = new PlayerDTO
                {
                    Id = playerId,
                    X = 0,
                    Y = 0
                }
            };
        }
        public async Task WaitingForSecondPlayer(Guid id)
        {
            while (await _context.Sessions.Where(s => s.Id == id && s.Player2Id == null).AnyAsync())
                await Task.Delay(1000);
        }
        public async Task WaitForRoundEnd(Guid id)
        {
            while (await _context.Sessions.Include(s => s.Player1).Include(s => s.Player2).Where(s => s.Id == id && (s.Player1.PlayedTurn == true && s.Player2!.PlayedTurn == true)).AnyAsync())
                await Task.Delay(1000);
        }
        public async Task<Session?> GetSessionByIdAsync(Guid id)
            => await _context.Sessions.Include(s => s.Player1).Include(s => s.Player2).AsNoTracking().Where(s => s.Id == id).FirstOrDefaultAsync();
        public async Task<bool> IncrementRound(Guid id)
        {
            Session? session = await GetSessionByIdAsync(id);
            if (session == null)
                return false;

            session.Round++;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
