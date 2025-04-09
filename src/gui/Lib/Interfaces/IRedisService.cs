using System.Collections.Concurrent;
using StackExchange.Redis;

namespace UI.Lib.Interfaces
{
    public interface IRedisService
    {
        event EventHandler<string>? OnStateChanged;
        ConcurrentDictionary<string, RedisValue> State { get; }
        Task SetState(string key, RedisValue value);
        Task<RedisValue> GetState(string key);
        Task<bool> KeyExists(string key);
        Task DeleteKey(string key);
    }
}