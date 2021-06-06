using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RateApiLimiter.Domain;
using RateApiLimiter.Interfaces;
using RateApiLimiter.Utilities;

namespace RateApiLimiter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HotelController : ControllerBase
    {
        private readonly ILogger<HotelController> _logger;
        private readonly IHotelService _hotelService;

        public HotelController(ILogger<HotelController> logger, IHotelService hotelService)
        {
            _logger = logger;
            _hotelService = hotelService;
        }

        private IEnumerable<Hotel> GetHotelCommon(string? city, RoomType? roomType, ListSortDirection? priceSortDirection)
        {
            var queryResult = _hotelService.GetHotels(
                                                                        hotel => (!roomType.HasValue || roomType == hotel.RoomType)
                                                                        && (string.IsNullOrEmpty(city) || city == hotel.City)
                                                                    );

            return priceSortDirection switch
            {
                ListSortDirection.Ascending => queryResult.OrderBy(hotel => hotel.Price),
                ListSortDirection.Descending => queryResult.OrderByDescending(hotel => hotel.Price),
                _ => queryResult
            };
        }
        
        [HttpGet("")]
        public IEnumerable<Hotel> GetHotel([FromQuery] string? city, [FromQuery] RoomType? roomType, ListSortDirection? priceSortDirection) =>
            GetHotelCommon(city, roomType, priceSortDirection);
        
        [HttpGet("city")]
        public IEnumerable<Hotel> GetHotelInCity([FromQuery] string city, [FromQuery] ListSortDirection? priceSortDirection = null) =>
            GetHotelCommon(city, null, priceSortDirection);

        [HttpGet("room")]
        public IEnumerable<Hotel> GetHotelWithRoomType([FromQuery] RoomType roomType, [FromQuery] ListSortDirection? priceSortDirection = null) =>
            GetHotelCommon(null, roomType, priceSortDirection);
    }

}