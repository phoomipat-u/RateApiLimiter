using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using RateApiLimiter.Domain;
using RateApiLimiter.Interfaces;

namespace RateApiLimiter.Services
{
    public class HotelService : IHotelService
    {
        private readonly ILogger<HotelService> _logger;
        private readonly IStorage<Hotel> _hotelStorage;
        public HotelService(ILogger<HotelService> logger, IStorage<Hotel> hotelStorage)
        {
            _logger = logger;
            _hotelStorage = hotelStorage;
        }

        public IEnumerable<Hotel> GetHotels(Func<Hotel, bool> predicate) => _hotelStorage.Get(predicate);
        
        public IEnumerable<Hotel> GetHotelsInCity(string city) => _hotelStorage.Get(hotel => hotel.City == city);
        
        public IEnumerable<Hotel> GetHotelsWithRoomType(RoomType roomType) => _hotelStorage.Get(hotel => hotel.RoomType == roomType);
    }
}