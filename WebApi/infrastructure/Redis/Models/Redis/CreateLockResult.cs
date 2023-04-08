namespace infrastructure.Models.Redis
{
    public class CreateLockResult
    {
        public bool Success { get; set; }

        public string Value { get; set; }
    }
}
