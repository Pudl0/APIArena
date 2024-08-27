using APIArena.DTO;
using APIArena.Models;
using APIArena.Server;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace APIArena.Services
{
    public class SessionService(DataContext _context, PlayerService _playerService, MapService _mapService)
    {
        public async Task<GameDTO> JoinOrCreatePvPSessionAsync(Player player, MapDTO map)
        {
            List<Session> openSession = await _context.Sessions
                .Where(s => s.Player2Id == null).ToListAsync();

            if (openSession.Count > 0)
                return await JoinSessionAsync(openSession.First().Id, player, map);

            var session = await CreateSessionAsync(player.Id, map.Id, GameDTO.GameMode.PvP);
            await WaitingForSecondPlayer(session.Id);

            session = await GetSessionByIdAsync(session.Id);
            if (session!.Player2Id == null)
            {
                throw new Exception("Session Error");
            }

            return new GameDTO
            {
                Id = session.Id,
                Player = new PlayerDTO
                {
                    Id = player.Id,
                    X = 0,
                    Y = 0
                },
                Enemy = new PlayerDTO
                {
                    Id = (Guid)session.Player2Id,
                    X = map.Width - 1,
                    Y = map.Height - 1
                }
            };
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
                },
                Enemy = new PlayerDTO
                {
                    Id = (Guid)session.Player1Id,
                    X = 0,
                    Y = 0
                }
            };
        }
        private async Task<Session> CreateSessionAsync(Guid playerId, Guid mapId, GameDTO.GameMode gameMode)
        {
            Guid sessionId = Guid.NewGuid();

            Session? session = new()
            {
                Id = sessionId,
                Player1Id = playerId,
                MapId = mapId,
                Round = 0,
                Mode = gameMode
            };

            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();

            return session;
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
        public async Task EndGame(Session session)
        {
            await _playerService.DeletePlayerAsync(session.Player1Id);
            if (session.Player2Id is not null)
                await _playerService.DeletePlayerAsync((Guid)session.Player2Id);

            await _mapService.DeleteMapAsync(session.MapId);

            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync();
        }
        public async Task<GameDTO> JoinPvESessionAsync(Player player, MapDTO map)
        {
            Player botPlayer = await _playerService.CreatePlayerAsync("Bot", null);

            Session session = await CreateSessionAsync(botPlayer.Id, map.Id, GameDTO.GameMode.PvE);

            return await JoinSessionAsync(session.Id, player, map);
        }
        public async Task<GameDTO?> GetGameDtoByIdAsync(Guid id, Player player)
        {
            Session? session = await GetSessionByIdAsync(id);
            if (session == null)
                return null;

            Player enemy = session.Player1Id == player.Id ? session.Player2! : session.Player1;

            return new GameDTO
            {
                Id = session.Id,
                Player = new PlayerDTO
                {
                    Id = player.Id,
                    X = player.XPos,
                    Y = player.YPos
                },
                Enemy = new PlayerDTO
                {
                    Id = enemy.Id,
                    X = enemy.XPos,
                    Y = enemy.YPos
                }
            };
        }
    }
}
