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
    public class GameController(IApiKeyValidator _apiKeyValidator, SessionService _sessionService, MapService _mapService, PlayerService _playerService) : ControllerBase
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

                MapDTO map = await _mapService.InitializeMapAsync();

                Player player = await _playerService.CreatePlayerAsync("PlaceHolderName", apiKey);

                GameDTO game = await _sessionService.JoinOrCreateSessionAsync(player, map);

                map.UpdateBasePositions(player);
                await _mapService.UpdateMapAsync(map);

                return Ok(new { game, map });
            }

            return BadRequest();
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
                    //case "MineRessource":
                    //    _mapService.MineRessource(session, player);
                    //    break;
                    //case "StoreRessource":
                    //    _mapService.StoreRessource(session, player);
                    //    break;
                    default:
                        return BadRequest();
                }

                // if player1 or 2 has played his turn, increment the round
                if (session.Player1.PlayedTurn || session.Player2!.PlayedTurn)
                    await _sessionService.IncrementRound(session.Id);

                // end turn when this player has played
                await TurnEnd(player, session);

                return Ok();
            }

            return BadRequest();
        }
        private async Task TurnEnd(Player player, Session session)
        {
            player.PlayedTurn = true;
            await _playerService.UpdatePlayerAsync(player);
            await _sessionService.WaitForRoundEnd(session.Id);

            player.PlayedTurn = false;
            await _playerService.UpdatePlayerAsync(player);
        }
    }
}
