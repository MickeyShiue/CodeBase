namespace WebApi.Infrastructure.CustomResult
{
    public class ApiResult : ApiResult<object> { }

    public class ApiResult<T> where T : new()
    {
        public ApiStatusCode Code { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }

        public ApiResult()
        {
            Code = ApiStatusCode.Success;
            Data = default(T) ?? new T();
            Message = string.Empty;
        }
    }

    public enum ApiStatusCode
    {
        Success = 200,
        Created = 201,
        BadRequest = 400,
        Unauthorized = 401,
        Forbidden = 403,
        NotFound = 404,
        InternalServerError = 500
    }
}
