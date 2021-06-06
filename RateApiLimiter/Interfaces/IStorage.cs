using System;
using System.Collections.Generic;

namespace RateApiLimiter.Interfaces
{
    public interface IStorage<out T>
    {
        IEnumerable<T> Get(Func<T, bool> predicate);
    }
}