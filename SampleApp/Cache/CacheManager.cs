using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace SampleApp.Cache
{
    public class CacheManager
    {
        IMemoryCache _Cache;

        // If there are other types of caching needed (i.e., distributed vs in-memory) add constructor
        // overloads for those types, and then modify GetFromCache to use the correct one.
        // Since the CacheManager is instantiated in Program.cs, and injected into things like the
        // MVC controllers, the implementation details are kept isolated.
        public CacheManager(IMemoryCache Cache) 
        { 
            if (Cache is null) {  throw new ArgumentNullException($"The parameter '{nameof(Cache)}' needs to be specified"); }
            this._Cache = Cache;
        }

        public async Task<T?> GetFromCache<T>(String CacheKey, Func<Task<T>> IfNotFound)
        {
            T? Response;

            CacheKey = (CacheKey ?? "").Trim();
            if (CacheKey.Equals(String.Empty)) { throw new ArgumentNullException($"The parameter '{nameof(CacheKey)}' needs to be specified"); }

            Response = await this._Cache.GetOrCreateAsync<T>(CacheKey, async _ => await IfNotFound());

            return Response;
        }


        public String GenerateUniqueName(String CacheKeyPart1, String CacheKeyPart2)
        {
            CacheKeyPart1 = (CacheKeyPart1 ?? "").Trim();
            CacheKeyPart2 = (CacheKeyPart2 ?? "").Trim();

            if (CacheKeyPart1.Equals(String.Empty) && CacheKeyPart2.Equals(String.Empty)) { throw new ArgumentException("At least one parameter needs to have a valid value (something not null, not empty, and not blank)"); }

            if (!CacheKeyPart1.Equals(String.Empty) && !CacheKeyPart2.Equals(String.Empty)) { return $"{CacheKeyPart1}_{CacheKeyPart2}"; }
            if (!CacheKeyPart1.Equals(String.Empty)) { return CacheKeyPart1; }
            return CacheKeyPart2;
        }
    }
}
