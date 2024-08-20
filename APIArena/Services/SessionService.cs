using APIArena.DTO;
using APIArena.Models;
using APIArena.Server;
using Microsoft.EntityFrameworkCore;

namespace APIArena.Services
{
    public class SessionService(DataContext _context, PlayerService _playerService)
    {
        public async Task<GameDTO> JoinOrCreateSessionAsync()
        {
            List<Session> openSession = await _context.Sessions
                .Where(s => s.Player2Id == null).ToListAsync();

            if (openSession.Count > 0)
                return await JoinSessionAsync(openSession.First().Id);

            return await CreateSessionAsync();
        }
        private async Task<GameDTO> JoinSessionAsync(Guid id)
        {
            Guid PlayerId = await _playerService.CreatePlayerAsync("Player2");
            Session? session = await _context.Sessions.Where(s => s.Id == id).FirstOrDefaultAsync();
            session!.Player2Id = PlayerId;
            _context.Sessions.Update(session);
            await _context.SaveChangesAsync();

            return new()
            {
                Id = session.Id,
                PlayerId = PlayerId,
                EnemyId = session.Player1Id
            };
        }
        private async Task<GameDTO> CreateSessionAsync()
        {
            Guid PlayerId = await _playerService.CreatePlayerAsync("Player1");
            Session? session = new()
            {
                Id = Guid.NewGuid(),
                Player1Id = PlayerId,
                ArenaId = Guid.NewGuid(),
                Round = 0
            };

            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();

            await WaitingForSecondPlayer(session.Id);
            session = await GetSessionByIdAsync(session.Id);

            if (session!.Player2Id == null)
                throw new Exception("Session Error");

            return new()
            {
                Id = session!.Id,
                PlayerId = PlayerId,
                EnemyId = (Guid)session.Player2Id
            };
        }
        private async Task WaitingForSecondPlayer(Guid id)
        {
            while (await _context.Sessions.Where(s => s.Id == id && s.Player2Id == null).AnyAsync())
                await Task.Delay(1000);
        }
        private async Task<Session?> GetSessionByIdAsync(Guid id)
            => await _context.Sessions.AsNoTracking().Where(s => s.Id == id).FirstOrDefaultAsync();
    }
}
