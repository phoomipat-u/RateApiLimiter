using System;
using System.Collections.Generic;
using System.Linq;
using RateApiLimiter.Domain;
using RateApiLimiter.Interfaces;

namespace UnitTest.Mocks
{
    public class MockHotelStorage : IStorage<Hotel>
    {
        private readonly IEnumerable<Hotel> hotelData;
        public MockHotelStorage(IEnumerable<Hotel> initializationData)
        {
            hotelData = initializationData;
        }

        public IEnumerable<Hotel> Get(Func<Hotel, bool> predicate) => hotelData.Where(predicate);
    }
}