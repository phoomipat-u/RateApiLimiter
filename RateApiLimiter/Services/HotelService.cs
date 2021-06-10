using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

        public IEnumerable<Hotel> GetHotels(Func<Hotel, bool> predicate, ListSortDirection? priceSortDirection)
        {
            var queryResult = _hotelStorage.Get(predicate);
            return priceSortDirection switch
            {
                ListSortDirection.Ascending => queryResult.OrderBy(hotel => hotel.Price),
                ListSortDirection.Descending => queryResult.OrderByDescending(hotel => hotel.Price),
                _ => queryResult
            };
        }
    }
}