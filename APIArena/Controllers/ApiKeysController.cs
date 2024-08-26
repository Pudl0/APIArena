using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIArena.Models;
using APIArena.Server;
using Tellerando.Infrastructure.Services;
using APIArena.Attributes;

namespace APIArena.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKeyRequired(Scopes = ["Admin"])]
    [RequireHttps]
    public class ApiKeysController(ApiKeyService _apiKeyService) : ControllerBase
    {

        // GET: api/ApiKeys
        [HttpGet]
        public ActionResult<IEnumerable<ApiKey>> GetApiKeys()
        {
            return _apiKeyService.GetAllKeysSortedByName();
        }

        // GET: api/ApiKeys/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiKey>> GetApiKey(Byte[] id)
        {
            ApiKey? apiKey = await _apiKeyService.GetApiKeyByIdAsync(id);

            if (apiKey == null)
                return NotFound();

            return apiKey;
        }

        // POST: api/ApiKeys
        [HttpPost]
        public ActionResult<String> PostApiKey(String name, String scopes)
        {
            String apiKey = _apiKeyService.CreateKey(name, scopes);

            if (String.IsNullOrEmpty(apiKey))
                return BadRequest();

            return apiKey;
        }

        // DELETE: api/ApiKeys/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Boolean>> DeleteApiKey(Byte[] id)
        {
            Boolean success = await _apiKeyService.DeleteKeyById(id);

            if (!success)
                return NotFound();

            return success;
        }
    }
}
