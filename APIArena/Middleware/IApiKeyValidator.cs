namespace APIArena.Middleware
{
    public interface IApiKeyValidator
    {
        bool IsValid(string apiKey, string[]? scopes, bool requiresAllScopes);
    }
}
