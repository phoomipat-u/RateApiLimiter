namespace Common.Interfaces
{
    public interface IRateLimiterService
    {
        bool RateLimitApi(string path);
    }
}