using APIArena.Attributes;
using APIArena.Services;
using Microsoft.AspNetCore.Mvc;
using APIArena.DTO;
using APIArena.Models;
using Microsoft.Extensions.Primitives;
using APIArena.Middleware;

namespace APIArena.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [ApiKeyRequired(Scopes = ["Game"])]
    [RequireHttps]
    public class GameController(IApiKeyValidator _apiKeyValidator, SessionService _sessionService, MapService _mapService, PlayerService _playerService, SettingService _settingService) : ControllerBase
    {
        [HttpPost("newgame")]
        public async Task<IActionResult> NewGame()
        {
            if (this.HttpContext.Request.Headers.TryGetValue("X-API-Key", out var rawApiKey))
            {
                // there can be more headers with the same name
                if (rawApiKey.Count != 1)
                    return BadRequest();

                // the key could not be valid
                ApiKey? apiKey = await _apiKeyValidator.GetSavedKeyFromInputAsync(rawApiKey!);
                if (apiKey == null)
                    return Unauthorized();

                GameDTO.GameMode gameMode = _settingService.GetSetting<GameDTO.GameMode>("GameMode");

                switch (gameMode)
                {
                    case GameDTO.GameMode.PvP:
                        return await NewPvPGame(apiKey);
                    case GameDTO.GameMode.PvE:
                        return await NewPvEGame(apiKey);
                    default:
                        return BadRequest();
                }
            }

            return BadRequest();
        }

        private async Task<IActionResult> NewPvPGame(ApiKey apiKey)
        {
            MapDTO map = await _mapService.InitializeMapAsync();

            Player player = await _playerService.CreatePlayerAsync("PlaceHolderName", apiKey);

            GameDTO game = await _sessionService.JoinOrCreatePvPSessionAsync(player, map);

            map.UpdateBasePositions(player);
            await _mapService.UpdateMapAsync(map);

            return Ok(new { game, map });
        }

        private async Task<IActionResult> NewPvEGame(ApiKey apiKey)
        {
            MapDTO map = await _mapService.InitializeMapAsync();

            Player player = await _playerService.CreatePlayerAsync(apiKey.Name, apiKey);

            GameDTO game = await _sessionService.JoinPvESessionAsync(player, map);

            map.UpdateBasePositions(player);
            await _mapService.UpdateMapAsync(map);

            return Ok(new { game, map });
        }

        [HttpPost("play")]
        public async Task<IActionResult> Play(Guid gameId, string action)
        {
            if (this.HttpContext.Request.Headers.TryGetValue("X-API-Key", out var rawApiKey))
            {
                // there can be more headers with the same name
                if (rawApiKey.Count != 1)
                    return BadRequest();

                // the key could not be valid
                ApiKey? apiKey = await _apiKeyValidator.GetSavedKeyFromInputAsync(rawApiKey!);
                if (apiKey == null)
                    return Unauthorized();

                // the game could not exist
                Session? session = await _sessionService.GetSessionByIdAsync(gameId);
                if (session == null)
                {
                    return NotFound();
                }

                switch (session.Mode)
                {
                    case GameDTO.GameMode.PvP:
                        return await PlayPvPGame(apiKey, session, action);
                    case GameDTO.GameMode.PvE:
                        return await PlayPvEGame(apiKey, session, action);
                    default:
                        return BadRequest();
                }
            }

            return BadRequest();
        }
        private async Task<IActionResult> PlayPvPGame(ApiKey apiKey, Session session, string action)
        {
            Player? player = await _playerService.GetPlayerByApiKeyAndSessionAsync(apiKey, session);

            // If the player has already played his turn and makes another request to play the turn this error will be returned
            if (player.PlayedTurn)
            {
                ModelState.AddModelError("Turn", "You have already played your current Turn");
                return BadRequest(ModelState);
            }

            MapDTO? map = await _mapService.GetMapDTOByIdAsync(session.MapId);
            if (map == null)
                return NotFound();

            switch (action)
            {
                case "MoveTop":
                    if (_playerService.MoveTop(player, map) is null)
                    {
                        ModelState.AddModelError("Move", "Invalid Move");
                        await TurnEnd(player, session);
                        return BadRequest(ModelState);
                    }
                    break;
                case "MoveBottom":
                    if (_playerService.MoveBottom(player, map) is null)
                    {
                        ModelState.AddModelError("Move", "Invalid Move");
                        await TurnEnd(player, session);
                        return BadRequest(ModelState);
                    }
                    break;
                case "MoveLeft":
                    if (_playerService.MoveLeft(player, map) is null)
                    {
                        ModelState.AddModelError("Move", "Invalid Move");
                        await TurnEnd(player, session);
                        return BadRequest(ModelState);
                    }
                    break;
                case "MoveRight":
                    if (_playerService.MoveRight(player, map) is null)
                    {
                        ModelState.AddModelError("Move", "Invalid Move");
                        await TurnEnd(player, session);
                        return BadRequest(ModelState);
                    }
                    break;
                case "MineRessource":
                    if (!await _playerService.MineRessourceAsync(player, map))
                    {
                        ModelState.AddModelError("Mine", "Invalid Mine");
                        await TurnEnd(player, session);
                        return BadRequest(ModelState);
                    }
                    break;
                case "StoreRessource":
                    if (!await _playerService.StoreRessourceAsync(player, map))
                    {
                        ModelState.AddModelError("Store", "Invalid Store");
                        await TurnEnd(player, session);
                        return BadRequest(ModelState);
                    }
                    break;
                default:
                    return BadRequest();
            }

            // if player1 or 2 has played his turn, increment the round
            if (session.Player1.PlayedTurn || session.Player2!.PlayedTurn)
                await _sessionService.IncrementRound(session.Id);

            // end turn when this player has played
            await TurnEnd(player, session);

            if (EndGameConditionReached(map, session))
            {
                await _sessionService.EndGame(session);

                if (player.Gold > session.Player1.Gold)
                    return Ok(new { message = "You Win" });
                else
                    return Ok(new { message = "You Loose" });
            }

            GameDTO? game = await _sessionService.GetGameDtoByIdAsync(session.Id, player);
            MapDTO? mapDTO = await _mapService.GetMapDTOByIdAsync(session.MapId);

            if (game is null || mapDTO is null)
                return NotFound();

            return Ok(new { session, map});
        }
        private async Task<IActionResult> PlayPvEGame(ApiKey apiKey, Session session, string action)
        {
            Player? player = await _playerService.GetPlayerByApiKeyAndSessionAsync(apiKey, session);

            // If the player has already played his turn and makes another request to play the turn this error will be returned
            if (player.PlayedTurn)
            {
                ModelState.AddModelError("Turn", "You have already played your current Turn");
                return BadRequest(ModelState);
            }

            MapDTO? map = await _mapService.GetMapDTOByIdAsync(session.MapId);
            if (map == null)
                return NotFound();

            switch (action)
            {
                case "MoveTop":
                    if (_playerService.MoveTop(player, map) is null)
                    {
                        ModelState.AddModelError("Move", "Invalid Move");
                        await TurnEnd(player, session);
                        return BadRequest(ModelState);
                    }
                    break;
                case "MoveBottom":
                    if (_playerService.MoveBottom(player, map) is null)
                    {
                        ModelState.AddModelError("Move", "Invalid Move");
                        await TurnEnd(player, session);
                        return BadRequest(ModelState);
                    }
                    break;
                case "MoveLeft":
                    if (_playerService.MoveLeft(player, map) is null)
                    {
                        ModelState.AddModelError("Move", "Invalid Move");
                        await TurnEnd(player, session);
                        return BadRequest(ModelState);
                    }
                    break;
                case "MoveRight":
                    if (_playerService.MoveRight(player, map) is null)
                    {
                        ModelState.AddModelError("Move", "Invalid Move");
                        await TurnEnd(player, session);
                        return BadRequest(ModelState);
                    }
                    break;
                case "MineRessource":
                    if (!await _playerService.MineRessourceAsync(player, map))
                    {
                        ModelState.AddModelError("Mine", "Invalid Mine");
                        await TurnEnd(player, session);
                        return BadRequest(ModelState);
                    }
                    break;
                case "StoreRessource":
                    if (!await _playerService.StoreRessourceAsync(player, map))
                    {
                        ModelState.AddModelError("Store", "Invalid Store");
                        await TurnEnd(player, session);
                        return BadRequest(ModelState);
                    }
                    break;
                default:
                    return BadRequest();
            }

            // get the bot player
            Player? botPlayer = await _playerService.GetPlayerByIdAsync(session.Player1.Id);
            if (botPlayer == null) {
                return NotFound();
            }

            player.PlayedTurn = true;
            await _playerService.UpdatePlayerAsync(player);

            // bot player plays
            await _playerService.BotPlayAsync(botPlayer, map, player);
            await _sessionService.IncrementRound(session.Id);

            player.PlayedTurn = false;
            await _playerService.UpdatePlayerAsync(player);

            if (EndGameConditionReached(map, session))
            {
                await _sessionService.EndGame(session);

                if (player.Gold > session.Player1.Gold)
                    return Ok(new { message = "You Win" });
                else
                    return Ok(new { message = "You Loose" });
            }

            GameDTO? game = await _sessionService.GetGameDtoByIdAsync(session.Id, player);
            MapDTO? mapDTO = await _mapService.GetMapDTOByIdAsync(session.MapId);

            if (game is null || mapDTO is null)
                return NotFound();

            return Ok(new { session, map });
        }
        private async Task TurnEnd(Player player, Session session)
        {
            player.PlayedTurn = true;
            await _playerService.UpdatePlayerAsync(player);
            await _sessionService.WaitForRoundEnd(session.Id);

            player.PlayedTurn = false;
            await _playerService.UpdatePlayerAsync(player);
        }
        private static bool EndGameConditionReached(MapDTO map, Session session)
        {
            if (map.IsGoldRemaining())
                return true;

            if (session.Round >= 50)
                return true;

            return false;
        }
    }
}
