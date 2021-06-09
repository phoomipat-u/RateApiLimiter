using System.Text.Json.Serialization;

namespace RateApiLimiter.Domain
{
    public class Hotel
    {
        // DO NOT REMOVE - Used when reading from CSV
        public Hotel()
        {
            
        }
        public Hotel(int id, string city, RoomType roomType, decimal price)
        {
            Id = id;
            City = city;
            RoomType = roomType;
            Price = price;
        }

        public int Id { get; set; }
        public string City { get; set; }
        
        public RoomType RoomType { get; set; }
        public decimal Price { get; set; }
    }
}