using infrastructure.Models.Redis;
using infrastructure.Redis.Enum;
using StackExchange.Redis;

namespace infrastructure.Redis
{
    public class RedisHelper : IRedisHelper
    {
        private const string UnLocKScriptNotCheckValue = @"
        if redis.call(""get"",KEYS[1]) then
            return redis.call(""del"",KEYS[1])
        else
            return 0
        end";

        private const string UnLocKScriptCheckValue = @"
        if redis.call(""get"",KEYS[1]) == ARGV[1] then
            return redis.call(""del"",KEYS[1])
        else
            return 0
        end";

        private readonly Lazy<ConnectionMultiplexer> _connection;
        private int redisDb => (int)RedisDb.DefaultDb;

        public ConnectionMultiplexer GetConnection() => _connection.Value;

        public RedisHelper()
        {
            const string connectionString = "localhost:6379";
            var options = ConfigurationOptions.Parse(connectionString);
            _connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options));
        }

        public async Task<CreateLockResult> CreateLockAsync(string key, string value, TimeSpan expiry)
        {
            var result = new CreateLockResult() { Success = true };

            var redisValue = await GetConnection().GetDatabase(redisDb)
                .StringSetAndGetAsync(key: key, value: value, expiry: expiry, When.NotExists, CommandFlags.None);

            if (redisValue.HasValue && redisValue.ToString() != value)
            {
                result.Success = false;
                result.Value = redisValue.ToString();
            }

            return result;
        }

        public async Task UnlockAsync(string key, string value = "")
        {
            RedisKey[] keys = { $"{key}" };

            if (!string.IsNullOrEmpty(value))
            {
                RedisValue[] values = { value };
                await GetConnection().GetDatabase(redisDb)
                    .ScriptEvaluateAsync(UnLocKScriptCheckValue, keys: keys, values: values);
            }
            else
            {
                await GetConnection().GetDatabase(redisDb)
                    .ScriptEvaluateAsync(UnLocKScriptNotCheckValue, keys: keys);
            }
        }
    }
}