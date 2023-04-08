using infrastructure.Redis;

namespace WebApi.Works
{
    public class RedisKeyExpiredEvent : BackgroundService
    {
        private readonly ILogger<RedisKeyExpiredEvent> _logger;
        private readonly IRedisHelper _redisHelper;
        private const string ExpiredKeysChannel = "__keyevent@0__:expired";

        public RedisKeyExpiredEvent(ILogger<RedisKeyExpiredEvent> logger, IRedisHelper redisHelper)
        {
            _logger = logger;
            _redisHelper = redisHelper;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("BackgroundService Start");

            var connection = _redisHelper.GetConnection();
            var subscriber = connection.GetSubscriber();
            await subscriber.SubscribeAsync(ExpiredKeysChannel, (channel, key) =>
                {
                    _logger.LogInformation($"RedisKey: {key} Expired");
                    // DoSomething
                }
            );
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("BackgroundService Stop");
            await Task.CompletedTask;
        }
    }
}
