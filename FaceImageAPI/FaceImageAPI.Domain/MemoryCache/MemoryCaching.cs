using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceImageAPI.Domain.MemoryCache
{
    public class MemoryCaching : ICacheProvider
    {
        private readonly IMemoryCache cache;

        public MemoryCaching(IMemoryCache cache)
        {
            this.cache = cache;  
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public object Get(string cacheKey)
        {
            return this.cache.Get(cacheKey);
        }

        /// <summary>
        /// Set
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheValue"></param>
        public void Set(string cacheKey, string cacheValue)
        {
            this.cache.Set(cacheKey, cacheValue, TimeSpan.FromSeconds(7200));
        }
    }
}
