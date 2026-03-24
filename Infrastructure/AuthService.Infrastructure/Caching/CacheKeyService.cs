namespace AuthService.Infrastructure.Caching;

using AuthService.Application.Common.ApplicationServices.Caching;


public class CacheKeyService : ICacheKeyService
{
    public string GetCacheKey(string name, object id)
    {
        return $"{name}-{id}";
    }
}