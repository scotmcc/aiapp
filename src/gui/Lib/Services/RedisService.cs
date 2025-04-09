using System.Collections.Concurrent;
using StackExchange.Redis;
using UI.Lib.Interfaces;

namespace UI.Lib.Services
{
    public class RedisService(ConnectionMultiplexer redis) : IRedisService
    {
        public event EventHandler<string>? OnStateChanged;
        private readonly ConnectionMultiplexer _redis = redis;
        public ConcurrentDictionary<string, RedisValue> State { get; private set; } = new ConcurrentDictionary<string, RedisValue>();
        public async Task SetState(string key, RedisValue value)
        {
            var db = _redis.GetDatabase();
            State[key] = value;
            await db.StringSetAsync(key, value);
            OnStateChanged?.Invoke(this, key);
        }
        public async Task<RedisValue> GetState(string key)
        {
            if (State.TryGetValue(key, out RedisValue value))
            {
                return value;
            }
            var db = _redis.GetDatabase();
            var result = await db.StringGetAsync(key);
            if (!result.IsNullOrEmpty)
            {
                State[key] = result;
            }
            return result;
        }
        public async Task<bool> KeyExists(string key)
        {
            if (State.ContainsKey(key))
            {
                return true;
            }
            var db = _redis.GetDatabase();
            return await db.KeyExistsAsync(key);
        }
        public async Task DeleteKey(string key)
        {
            var db = _redis.GetDatabase();
            if (await db.KeyDeleteAsync(key))
            {
                State.TryRemove(key, out _);
                OnStateChanged?.Invoke(this, key);
            }
        }
    }
}
