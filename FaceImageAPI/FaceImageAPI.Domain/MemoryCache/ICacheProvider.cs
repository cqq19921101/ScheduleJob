using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceImageAPI.Domain.MemoryCache
{
    /// <summary>
    /// Simple Cache Interface  Include : Get Set 
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        /// Get 
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        object Get(string cacheKey);

        /// <summary>
        /// Set
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="cacheValue"></param>
        void Set(string cacheKey, string cacheValue);

    }
}
