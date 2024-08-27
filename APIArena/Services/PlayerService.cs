using APIArena.DTO;
using APIArena.Models;
using APIArena.Server;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace APIArena.Services
{
    public class PlayerService(DataContext _context, MapService _mapService)
    {
        public async Task<Player> CreatePlayerAsync(string name, ApiKey? apiKey)
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
                ApiKeyId = apiKey == null ? null : apiKey.Id
            };

            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            return player;
        }

        public async Task<Player?> GetPlayerByIdAsync(Guid id)
        {
            return await _context.Players.FindAsync(id);
        }

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

        public async Task<bool> MineRessourceAsync(Player player, MapDTO map)
        {
            if (!(map.Tiles[player.XPos][player.YPos].Type == TileDTO.TileType.Gold))
                return false;

            player.Gold++;
            map.Tiles[player.XPos][player.YPos].Type = TileDTO.TileType.Empty;
            await UpdatePlayerAsync(player);
            await _mapService.UpdateMapAsync(map);
            return true;
        }

        public async Task<bool> StoreRessourceAsync(Player player, MapDTO map)
        {
            if (!(map.Tiles[player.XPos][player.YPos].Type == TileDTO.TileType.Base))
                return false;

            player.Gold--;
            player.Level++;
            await UpdatePlayerAsync(player);
            return true;
        }

        public async Task<bool> DeletePlayerAsync(Guid id)
        {
            Player? player = await _context.Players.FirstOrDefaultAsync(p => p.Id == id);
            if (player == null)
                return false;

            _context.Players.Remove(player);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task BotPlayAsync(Player botPlayer, MapDTO map, Player player)
        {
            if (map.Tiles[botPlayer.XPos][botPlayer.YPos].Type == TileDTO.TileType.Gold)
            {
                await MineRessourceAsync(botPlayer, map);
            }
            else if (botPlayer.Gold == 0)
            {
                // Find the nearest gold tile
                TileDTO.TileType targetTileType = TileDTO.TileType.Gold;
                List<TileDTO> goldTiles = map.Tiles.SelectMany(row => row).Where(tile => tile.Type == targetTileType).ToList();
                TileDTO? nearestGoldTile = goldTiles.OrderBy(tile => Math.Abs(tile.X - botPlayer.XPos) + Math.Abs(tile.Y - botPlayer.YPos)).FirstOrDefault();

                if (nearestGoldTile != null)
                {
                    if (nearestGoldTile.X > botPlayer.XPos)
                    {
                        await MoveRight(botPlayer, map);
                    }
                    else if (nearestGoldTile.X < botPlayer.XPos)
                    {
                        await MoveLeft(botPlayer, map);
                    }
                    else if (nearestGoldTile.Y > botPlayer.YPos)
                    {
                        await MoveTop(botPlayer, map);
                    }
                    else if (nearestGoldTile.Y < botPlayer.YPos)
                    {
                        await MoveBottom(botPlayer, map);
                    }
                }
            }
            else
            {
                // Find the base tile
                TileDTO.TileType targetTileType = TileDTO.TileType.Base;
                List<TileDTO> baseTiles = map.Tiles.SelectMany(row => row).Where(tile => tile.Type == targetTileType).ToList();
                TileDTO? baseTile = baseTiles.FirstOrDefault();

                if (baseTile != null)
                {
                    if (baseTile.X > botPlayer.XPos)
                    {
                        await MoveRight(botPlayer, map);
                    }
                    else if (baseTile.X < botPlayer.XPos)
                    {
                        await MoveLeft(botPlayer, map);
                    }
                    else if (baseTile.Y > botPlayer.YPos)
                    {
                        await MoveTop(botPlayer, map);
                    }
                    else if (baseTile.Y < botPlayer.YPos)
                    {
                        await MoveBottom(botPlayer, map);
                    }
                    else
                    {
                        await StoreRessourceAsync(botPlayer, map);
                    }
                }
            }
        }
    }
}
