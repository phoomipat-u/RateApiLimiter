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
    public class GetHotelsByCityTest
    {
        [Fact]
        public void GivenHotelStorageIsEmpty_WhenCallingGetHotelByCityWithBangkokAsInput_ThenRetrieveResultIsEmpty()
        {
            // Arrange
            var hotels = System.Array.Empty<Hotel>();
            var mockHotelServiceLogger = new Mock<ILogger<HotelService>>();
            var mockHotelStorage = new MockHotelStorage(hotels);
            var hotelService = new HotelService(mockHotelServiceLogger.Object, mockHotelStorage);
            
            var mockHotelControllerLogger = new Mock<ILogger<RateApiLimiter.Controllers.HotelController>>();
            var hotelController = new RateApiLimiter.Controllers.HotelController(mockHotelControllerLogger.Object, hotelService);
            
            // Act
            var result = hotelController.GetHotelInCity("Bangkok");
            
            // Assert
            Assert.Empty(result);
        }
        
        [Fact]
        public void GivenASingleHotelInBangkokInStorage_WhenCallingGetHotelByCityWithBangkokAsInput_ThenRetrieveResultIsOneHotelInBangkok()
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
            var result = hotelController.GetHotelInCity("Bangkok");
            
            // Assert
            Assert.All(result, hotel => Assert.Equal("Bangkok", hotel.City));
        }
        
        [Fact]
        public void GivenListOfHotelsFromDifferentLocationsIncludingBangkokInStorage_WhenCallingGetHotelByCityWithBangkokAsInput_ThenRetrieveResultOnlyContainsBangkokHotels()
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
            var result = hotelController.GetHotelInCity("Bangkok");
            
            // Assert
            Assert.All(result, hotel => Assert.Equal("Bangkok", hotel.City));
        }
        
        [Fact]
        public void GivenAListOfHotelsThatDoesNotIncludeBangkokHotelsInStorage_WhenCallingGetHotelByCityWithBangkokAsInput_ThenRetrieveResultIsEmpty()
        {
            // Arrange

            var hotels = new[]
            {
                new Hotel(1, "Tokyo", RoomType.Deluxe, 100),
                new Hotel(2, "London", RoomType.SweetSuite, 100),
                new Hotel(3, "Miami", RoomType.Superior, 200)
            };
            var mockHotelServiceLogger = new Mock<ILogger<HotelService>>();
            var mockHotelStorage = new MockHotelStorage(hotels);
            var hotelService = new HotelService(mockHotelServiceLogger.Object, mockHotelStorage);
            
            var mockHotelControllerLogger = new Mock<ILogger<RateApiLimiter.Controllers.HotelController>>();
            var hotelController = new RateApiLimiter.Controllers.HotelController(mockHotelControllerLogger.Object, hotelService);
            
            // Act
            var result = hotelController.GetHotelInCity("Bangkok");
            
            // Assert
            Assert.Empty(result);
        }
        
        [Fact]
        public void GivenAListOfBangkokHotelsInStorage_WhenCallingGetHotelByCityWithBangkokAsInputAndSortPriceAsUnspecified_ThenRetrieveResultShouldHaveSameOrderAsInStorage()
        {
            // Arrange

            var hotels = new[]
            {
                new Hotel(1, "Bangkok", RoomType.Deluxe, 500),
                new Hotel(2, "Bangkok", RoomType.SweetSuite, 50),
                new Hotel(3, "Bangkok", RoomType.Superior, 200)
            };
            var mockHotelServiceLogger = new Mock<ILogger<HotelService>>();
            var mockHotelStorage = new MockHotelStorage(hotels);
            var hotelService = new HotelService(mockHotelServiceLogger.Object, mockHotelStorage);
            
            var mockHotelControllerLogger = new Mock<ILogger<RateApiLimiter.Controllers.HotelController>>();
            var hotelController = new RateApiLimiter.Controllers.HotelController(mockHotelControllerLogger.Object, hotelService);
            
            // Act
            var result = hotelController.GetHotelInCity("Bangkok");
            
            // Assert
            Assert.True(result.SequenceEqual(hotels));
        }
        
        [Fact]
        public void GivenAListOfBangkokHotelsInStorage_WhenCallingGetHotelByCityWithBangkokAsInputAndSortPriceAscending_ThenRetrieveResultIsSortedByPriceAscending()
        {
            // Arrange

            var hotels = new[]
            {
                new Hotel(1, "Bangkok", RoomType.Deluxe, 500),
                new Hotel(2, "Bangkok", RoomType.SweetSuite, 50),
                new Hotel(3, "Bangkok", RoomType.Superior, 200)
            };
            var mockHotelServiceLogger = new Mock<ILogger<HotelService>>();
            var mockHotelStorage = new MockHotelStorage(hotels);
            var hotelService = new HotelService(mockHotelServiceLogger.Object, mockHotelStorage);
            
            var mockHotelControllerLogger = new Mock<ILogger<RateApiLimiter.Controllers.HotelController>>();
            var hotelController = new RateApiLimiter.Controllers.HotelController(mockHotelControllerLogger.Object, hotelService);
            
            // Act
            var result = hotelController.GetHotelInCity("Bangkok", ListSortDirection.Ascending);
            
            // Assert
            Assert.True(result.SequenceEqual(hotels.OrderBy(hotel => hotel.Price)));
        }
        
        [Fact]
        public void GivenAListOfBangkokHotelsInStorage_WhenCallingGetHotelByCityWithBangkokAsInputAndSortPriceDescending_ThenRetrieveResultIsSortedByPriceDescending()
        {
            // Arrange

            var hotels = new[]
            {
                new Hotel(1, "Bangkok", RoomType.Deluxe, 500),
                new Hotel(2, "Bangkok", RoomType.SweetSuite, 50),
                new Hotel(3, "Bangkok", RoomType.Superior, 200)
            };
            var mockHotelServiceLogger = new Mock<ILogger<HotelService>>();
            var mockHotelStorage = new MockHotelStorage(hotels);
            var hotelService = new HotelService(mockHotelServiceLogger.Object, mockHotelStorage);
            
            var mockHotelControllerLogger = new Mock<ILogger<RateApiLimiter.Controllers.HotelController>>();
            var hotelController = new RateApiLimiter.Controllers.HotelController(mockHotelControllerLogger.Object, hotelService);
            
            // Act
            var result = hotelController.GetHotelInCity("Bangkok", ListSortDirection.Descending);
            
            // Assert
            Assert.True(result.SequenceEqual(hotels.OrderByDescending(hotel => hotel.Price)));
        }
    }
}