using Microsoft.EntityFrameworkCore;
using APIArena.Models;
using APIArena.Middleware;
using APIArena.Server;

namespace Tellerando.Infrastructure.Services
{
    public class ApiKeyService(DataContext _context, IDbContextFactory<DataContext> _dbContextFactory)
    {
        public List<ApiKey> GetAllKeysSortedByName()
        {
            using var context = _dbContextFactory.CreateDbContext();
            return [.. context.ApiKeys.OrderBy(k => k.Name)];
        }

        public async Task<ApiKey?> GetApiKeyByIdAsync(byte[] id)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.ApiKeys.FindAsync(id);
        }

        public string CreateKey(string name, string scopes = "")
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;

            try
            {
                (var model, var apiKey) = ApiKeyValidator.GenerateApiKey(name, scopes);

                _context.ApiKeys.Add(model);
                _context.SaveChanges();

                return apiKey;
            }
            catch (Exception) { }

            return string.Empty;
        }

        /// <summary>
        /// Changes the API Scopes for the specified ID. Leave empty to remove all scopes.
        /// </summary>
        /// <param name="id">The API key to change</param>
        /// <param name="scopes">The new scopes for the API key</param>
        /// <returns>An awaitable <see cref="Task"/> which resolves into a <see langword="bool"/> that indicates success.</returns>
        public async Task<bool> ChangeScopesOfKeyAsync(byte[] id, string? scopes = "")
            => 0 < await _context.ApiKeys.Where(x => x.Id.Equals(id)).ExecuteUpdateAsync(x => x.SetProperty(y => y.Scopes, scopes));

        public async Task<bool> ChangeNameOfKeyAsync(byte[] id, string? name = "")
            => 0 < await _context.ApiKeys.Where(x => x.Id.Equals(id)).ExecuteUpdateAsync(x => x.SetProperty(y => y.Name, name));

        public async Task<bool> DeleteKeyById(byte[] id)
        {
            if (id.Length <= 0)
                return false;

            try
            {
                return 0 < await _context.ApiKeys.Where(k => k.Id == id).ExecuteDeleteAsync();
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}