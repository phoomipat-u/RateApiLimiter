using System.ComponentModel;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using RateApiLimiter.Domain;
using RateApiLimiter.Services;
using UnitTest.Mocks;
using Xunit;

namespace UnitTest.Controller.HotelController
{
    public class GetHotelsWithRoomTypeTest
    {
        [Fact]
        public void GivenHotelStorageIsEmpty_WhenCallingGetHotelWithRoomTypeWithDeluxeAsInput_ThenRetrieveResultIsEmpty()
        {
            // Arrange
            var hotels = System.Array.Empty<Hotel>();
            var mockHotelServiceLogger = new Mock<ILogger<HotelService>>();
            var mockHotelStorage = new MockHotelStorage(hotels);
            var hotelService = new HotelService(mockHotelServiceLogger.Object, mockHotelStorage);
            
            var mockHotelControllerLogger = new Mock<ILogger<RateApiLimiter.Controllers.HotelController>>();
            var hotelController = new RateApiLimiter.Controllers.HotelController(mockHotelControllerLogger.Object, hotelService);
            
            // Act
            var result = hotelController.GetHotelWithRoomType(RoomType.Deluxe);
            
            // Assert
            Assert.Empty(result);
        }
        
        [Fact]
        public void GivenASingleDeluxeHotelInStorage_WhenCallingGetHotelWithRoomTypeAsDeluxe_ThenRetrieveResultIsOneDeluxeHotel()
        {
            // Arrange

            var hotels = new[]
            {
                new Hotel(1, "Bangkok", RoomType.Deluxe, 100),
            };
            var mockHotelServiceLogger = new Mock<ILogger<HotelService>>();
            var mockHotelStorage = new MockHotelStorage(hotels);
            var hotelService = new HotelService(mockHotelServiceLogger.Object, mockHotelStorage);
            
            var mockHotelControllerLogger = new Mock<ILogger<RateApiLimiter.Controllers.HotelController>>();
            var hotelController = new RateApiLimiter.Controllers.HotelController(mockHotelControllerLogger.Object, hotelService);
            
            // Act
            var result = hotelController.GetHotelWithRoomType(RoomType.Deluxe);
            
            // Assert
            Assert.All(result, hotel => Assert.Equal(RoomType.Deluxe, hotel.RoomType));
            Assert.Single(result);
        }
        
        [Fact]
        public void GivenListOfHotelsWithDifferentTypesOfRoomsIncludingDeluxeInStorage_WhenCallingGetHotelWithRoomTypeAsDeluxe_ThenRetrieveResultOnlyContainsDeluxeHotels()
        {
            // Arrange

            var hotels = new[]
            {
                new Hotel(1, "Bangkok", RoomType.Deluxe, 100),
                new Hotel(2, "Bangkok", RoomType.SweetSuite, 100),
                new Hotel(3, "Miami", RoomType.Superior, 200)
            };
            var mockHotelServiceLogger = new Mock<ILogger<HotelService>>();
            var mockHotelStorage = new MockHotelStorage(hotels);
            var hotelService = new HotelService(mockHotelServiceLogger.Object, mockHotelStorage);
            
            var mockHotelControllerLogger = new Mock<ILogger<RateApiLimiter.Controllers.HotelController>>();
            var hotelController = new RateApiLimiter.Controllers.HotelController(mockHotelControllerLogger.Object, hotelService);
            
            // Act
            var result = hotelController.GetHotelWithRoomType(RoomType.Deluxe);
            
            // Assert
            Assert.All(result, hotel => Assert.Equal(RoomType.Deluxe, hotel.RoomType));
        }
        
        [Fact]
        public void GivenAListOfHotelsThatDoesNotIncludeDeluxeHotelsInStorage_WhenCallingGetHotelWithRoomTypeAsDeluxe_ThenRetrieveResultIsEmpty()
        {
            // Arrange

            var hotels = new[]
            {
                new Hotel(1, "Tokyo", RoomType.Superior, 100),
                new Hotel(2, "London", RoomType.SweetSuite, 100),
                new Hotel(3, "Miami", RoomType.Superior, 200)
            };
            var mockHotelServiceLogger = new Mock<ILogger<HotelService>>();
            var mockHotelStorage = new MockHotelStorage(hotels);
            var hotelService = new HotelService(mockHotelServiceLogger.Object, mockHotelStorage);
            
            var mockHotelControllerLogger = new Mock<ILogger<RateApiLimiter.Controllers.HotelController>>();
            var hotelController = new RateApiLimiter.Controllers.HotelController(mockHotelControllerLogger.Object, hotelService);
            
            // Act
            var result = hotelController.GetHotelWithRoomType(RoomType.Deluxe);
            
            // Assert
            Assert.Empty(result);
        }
        
        [Fact]
        public void GivenAListOfDeluxeRoomHotelsInStorage_WhenCallingGetHotelWithRoomTypeAsDeluxeAndSortPriceAsUnspecified_ThenRetrieveResultShouldHaveSameOrderAsInStorage()
        {
            // Arrange

            var hotels = new[]
            {
                new Hotel(1, "Bangkok", RoomType.Deluxe, 500),
                new Hotel(2, "Bangkok", RoomType.Deluxe, 50),
                new Hotel(3, "Bangkok", RoomType.Deluxe, 200)
            };
            var mockHotelServiceLogger = new Mock<ILogger<HotelService>>();
            var mockHotelStorage = new MockHotelStorage(hotels);
            var hotelService = new HotelService(mockHotelServiceLogger.Object, mockHotelStorage);
            
            var mockHotelControllerLogger = new Mock<ILogger<RateApiLimiter.Controllers.HotelController>>();
            var hotelController = new RateApiLimiter.Controllers.HotelController(mockHotelControllerLogger.Object, hotelService);
            
            // Act
            var result = hotelController.GetHotelWithRoomType(RoomType.Deluxe);
            
            // Assert
            Assert.True(result.SequenceEqual(hotels));
        }
        
        [Fact]
        public void GivenAListOfDeluxeRoomHotelsInStorage_WhenCallingGetHotelWithRoomTypeAsDeluxeAndSortPriceAscending_ThenRetrieveResultIsSortedByPriceAscending()
        {
            // Arrange

            var hotels = new[]
            {
                new Hotel(1, "Bangkok", RoomType.Deluxe, 500),
                new Hotel(2, "Bangkok", RoomType.Deluxe, 50),
                new Hotel(3, "Bangkok", RoomType.Deluxe, 200)
            };
            var mockHotelServiceLogger = new Mock<ILogger<HotelService>>();
            var mockHotelStorage = new MockHotelStorage(hotels);
            var hotelService = new HotelService(mockHotelServiceLogger.Object, mockHotelStorage);
            
            var mockHotelControllerLogger = new Mock<ILogger<RateApiLimiter.Controllers.HotelController>>();
            var hotelController = new RateApiLimiter.Controllers.HotelController(mockHotelControllerLogger.Object, hotelService);
            
            // Act
            var result = hotelController.GetHotelWithRoomType(RoomType.Deluxe, ListSortDirection.Ascending);
            
            // Assert
            Assert.True(result.SequenceEqual(hotels.OrderBy(hotel => hotel.Price)));
        }
        
        [Fact]
        public void GivenAListOfDeluxeRoomHotelsInStorage_WhenCallingGetHotelWithRoomTypeAsDeluxeAndSortPriceDescending_ThenRetrieveResultIsSortedByPriceDescending()
        {
            // Arrange

            var hotels = new[]
            {
                new Hotel(1, "Bangkok", RoomType.Deluxe, 500),
                new Hotel(2, "Bangkok", RoomType.Deluxe, 50),
                new Hotel(3, "Bangkok", RoomType.Deluxe, 200)
            };
            var mockHotelServiceLogger = new Mock<ILogger<HotelService>>();
            var mockHotelStorage = new MockHotelStorage(hotels);
            var hotelService = new HotelService(mockHotelServiceLogger.Object, mockHotelStorage);
            
            var mockHotelControllerLogger = new Mock<ILogger<RateApiLimiter.Controllers.HotelController>>();
            var hotelController = new RateApiLimiter.Controllers.HotelController(mockHotelControllerLogger.Object, hotelService);
            
            // Act
            var result = hotelController.GetHotelWithRoomType(RoomType.Deluxe, ListSortDirection.Descending);
            
            // Assert
            Assert.True(result.SequenceEqual(hotels.OrderByDescending(hotel => hotel.Price)));
        }
    }
}