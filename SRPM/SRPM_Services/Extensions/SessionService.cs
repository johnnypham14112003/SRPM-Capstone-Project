using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPM_Services.Extensions
{
    public interface ISessionService
    {
        void Store(string sessionId, object data, TimeSpan expiration);
        object Retrieve(string sessionId);
        bool TryRemove(string sessionId);
    }

    public class MemorySessionService : ISessionService
    {
        private readonly IMemoryCache _cache;

        public MemorySessionService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void Store(string sessionId, object data, TimeSpan expiration)
        {
            _cache.Set(sessionId, data, expiration);
        }

        public object Retrieve(string sessionId)
        {
            return _cache.TryGetValue(sessionId, out var data) ? data : null;
        }

        public bool TryRemove(string sessionId)
        {
            _cache.Remove(sessionId);
            return true;
        }
    }

}
