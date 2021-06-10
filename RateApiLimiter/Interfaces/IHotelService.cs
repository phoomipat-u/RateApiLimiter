using System;
using System.Collections.Generic;
using System.ComponentModel;
using RateApiLimiter.Domain;

namespace RateApiLimiter.Interfaces
{
    public interface IHotelService
    {
        IEnumerable<Hotel> GetHotels(Func<Hotel, bool> predicate, ListSortDirection? priceSortDirection);
    }
}