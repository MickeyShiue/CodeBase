using infrastructure.Redis;
using Microsoft.AspNetCore.Mvc;
using WebApi.Infrastructure.CustomResult;

namespace WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RedisController : ControllerBase
    {
        private readonly IRedisHelper _redisHelper;
        public class TransReq
        {
            public string TransId { get; set; }
        }

        public RedisController(IRedisHelper redisHelper)
        {
            _redisHelper = redisHelper;
        }

        [HttpPost]
        public async Task<ApiResult> Lock(TransReq req)
        {
            var key = $"XX{req.TransId}"; //lock key
            var value = Guid.NewGuid().ToString();
            var expiry = TimeSpan.FromSeconds(10); // lock key expire 
            var createResult = await _redisHelper.CreateLockAsync(key, value, expiry);

            if (createResult.Success) // 確定取得 lock 所有權
            {
                return new ApiResult()
                {
                    Code = ApiStatusCode.Success
                };
            }

            return new ApiResult()
            {
                Code = ApiStatusCode.InternalServerError,
                Data = createResult.Value
            };
        }

        [HttpPost]
        public async Task<ApiResult> UnLock(TransReq req)
        {
            var key = $"XX{req.TransId}";
            await _redisHelper.UnlockAsync(key);

            return new ApiResult()
            {
                Code = ApiStatusCode.Success
            };
        }
    }
}
