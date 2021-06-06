using System;
using System.Collections.Generic;
using RateApiLimiter.Domain;

namespace RateApiLimiter.Interfaces
{
    public interface IHotelService
    {
        IEnumerable<Hotel> GetHotels(Func<Hotel, bool> predicate);
        IEnumerable<Hotel> GetHotelsInCity(string city);
        IEnumerable<Hotel> GetHotelsWithRoomType(RoomType roomType);
    }
}