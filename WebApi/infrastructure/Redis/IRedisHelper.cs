using infrastructure.Models.Redis;
using StackExchange.Redis;

namespace infrastructure.Redis
{
    public interface IRedisHelper
    {
        ConnectionMultiplexer GetConnection();

        Task<CreateLockResult> CreateLockAsync(string key, string value, TimeSpan expiry);

        Task UnlockAsync(string key, string value = "");
    }
}
