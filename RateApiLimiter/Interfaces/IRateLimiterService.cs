using System;

namespace Common.Interfaces
{
    public interface IRateLimiterService
    {
        bool AllowApiCall(DateTime now, string endpoint);
    }
}