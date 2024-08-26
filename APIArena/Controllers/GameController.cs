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
    public class GameController(IApiKeyValidator _apiKeyValidator, SessionService _sessionservice, MapService _mapService, PlayerService _playerService) : ControllerBase
    {
        [HttpPost("newgame")]
        public async Task<IActionResult> NewGame()
        {
            if (this.HttpContext.Request.Headers.TryGetValue("X-API-Key", out var rawApiKey))
            {
                if (rawApiKey.Count != 1)
                    return BadRequest();

                ApiKey? apiKey = await _apiKeyValidator.GetSavedKeyFromInputAsync(rawApiKey!);
                if (apiKey == null)
                    return Unauthorized();

                MapDTO map = _mapService.InitializeMap();

                Player player = await _playerService.CreatePlayerAsync("PlaceHolderName", apiKey);

                GameDTO game = await _sessionservice.JoinOrCreateSessionAsync(player, map);

                map.UpdateBasePositions(player);

                return Ok(new { game, map });
            }
            return BadRequest();
        }
        //[HttpPost("play")]
        //public async Task<IActionResult> Play(Guid gameId, Guid playerId, string action)
        //{
        //    Session? session = await _sessionservice.GetSessionByIdAsync(gameId);
        //    if (session == null)
        //    {
        //        return NotFound();
        //    }

        //    if (session.Player1Id != playerId && session.Player2Id != playerId)
        //    {
        //        return Unauthorized();
        //    }

        //    Player? player = await _playerService.GetPlayerByIdAsync(playerId);

        //    switch(action)
        //    {
        //        case "MoveTop":
        //            _mapService.MoveTop(session, player);
        //            break;
        //        case "MoveBottom":
        //            _mapService.MoveBottom(session, player);
        //            break;
        //        case "MoveLeft":
        //            _mapService.MoveLeft(session, player);
        //            break;
        //        case "MoveRight":
        //            _mapService.MoveRight(session, player);
        //            break;
        //        case "MineRessource":
        //            _mapService.MineRessource(session, player);
        //            break;
        //        case "StoreRessource":
        //            _mapService.StoreRessource(session, player);
        //            break;
        //        default:
        //            return BadRequest();
        //    }

        //    return Ok();
        //}
    }
}
