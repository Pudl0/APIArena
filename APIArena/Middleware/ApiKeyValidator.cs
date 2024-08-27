using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using APIArena.Models;
using APIArena.Server;

namespace APIArena.Middleware
{
    public class ApiKeyValidator(IDbContextFactory<DataContext> dbContextFactory) : IApiKeyValidator
    {
        private const int Iterations = 10000;
        private const int SaltSize = 16;

        public bool IsValid(string apiKey, string[]? scopes, bool requiresAllScopes)
        {
            if (string.IsNullOrEmpty(apiKey))
                return false;

            using var context = dbContextFactory.CreateDbContext();

            var scopeCount = scopes?.Length ?? 0;

            foreach (var key in context.ApiKeys.ToList())
            {
                using var deriveBytes = new Rfc2898DeriveBytes(apiKey, key.Salt, Iterations, HashAlgorithmName.SHA512);

                if (key.Id.SequenceEqual(deriveBytes.GetBytes(32)))
                {
                    if (scopeCount <= 0)
                        return true;

                    var keyScopes = key.Scopes.ToUpperInvariant().Split(";");
                    var applicableScopes = 0;

                    foreach (var scope in scopes!)
                    {
                        if (keyScopes.Contains(scope.ToUpperInvariant()))
                        {
                            applicableScopes++;

                            if (!requiresAllScopes)
                                return true;
                        }
                    }

                    if (applicableScopes == scopeCount)
                        return true;
                }
            }

            return false;
        }
        public async Task<ApiKey?> GetSavedKeyFromInputAsync(string apiKey)
        {
            using var context = dbContextFactory.CreateDbContext();

            foreach (var key in await context.ApiKeys.ToListAsync())
            {
                using var deriveBytes = new Rfc2898DeriveBytes(apiKey, key.Salt, Iterations, HashAlgorithmName.SHA512);

                if (key.Id.SequenceEqual(deriveBytes.GetBytes(32)))
                {
                    return key;
                }
            }

            return null;
        }
        public static (ApiKey, string key) GenerateApiKey(string name, string scopes = "")
        {
            var key = Guid.NewGuid().ToString();
            using var deriveBytes = new Rfc2898DeriveBytes(key, SaltSize, Iterations, HashAlgorithmName.SHA512);
            return (
                new()
                {
                    Id = deriveBytes.GetBytes(32),
                    Salt = deriveBytes.Salt,
                    Name = name,
                    Scopes = scopes
                },
                key
            );
        }
    }
}
