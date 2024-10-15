using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace SampleApp.Cache
{
    public class CacheManager
    {
        IMemoryCache _Cache;

        // If there are other types of caching needed (i.e., something not in memory) add constructor
        // overloads for those types, and then modify GetFromCache to use the correct one.
        // That allows downstream classes like controllers from having to know the details of the
        // implementation.
        public CacheManager(IMemoryCache Cache) 
        { 
            // TODO: Validate this
            this._Cache = Cache;
        }

        public async Task<T> GetFromCache<T>(String CacheKey, Func<Task<T>> IfNotFound)
        {
            T Response;

            // TODO: Deal with the nullable here
            Response = await this._Cache.GetOrCreateAsync<T>(CacheKey, async _ => await IfNotFound());

            return Response;
        }
    }
}
