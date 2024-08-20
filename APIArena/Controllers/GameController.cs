using APIArena.Attributes;
using APIArena.Services;
using Microsoft.AspNetCore.Mvc;
using APIArena.DTO;

namespace APIArena.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [ApiKeyRequired(Scopes = ["Game"])]
    [RequireHttps]
    public class GameController(SessionService _sessionservice, MapService _mapService) : ControllerBase
    {
        [HttpPost("newgame")]
        public async Task<IActionResult> NewGame()
        {
            MapDTO map = _mapService.InitializeMap();
            GameDTO game = await _sessionservice.JoinOrCreateSessionAsync();
            return Ok(new { game, map });
        }
        //[HttpPost("play")]
        //public async Task<IActionResult> Play()
        //{
        //    GameDTO game = await _sessionservice.PlayAsync(play);
        //    return Ok(game);
        //}
    }
}
