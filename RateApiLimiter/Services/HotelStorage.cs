using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using RateApiLimiter.Domain;
using RateApiLimiter.Interfaces;
using static System.Enum;

namespace RateApiLimiter.Services
{
    public class HotelStorage : IStorage<Hotel>
    {
        private readonly ILogger<HotelStorage> _logger;
        private readonly IEnumerable<Hotel> _hotelCollection;
        
        public HotelStorage(ILogger<HotelStorage> logger)
        {
            _logger = logger;

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/HotelData.csv");
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<HotelCsvMap>();
            _hotelCollection = csv.GetRecords<Hotel>().ToArray();
        }

        public IEnumerable<Hotel> Get(Func<Hotel, bool> predicate) => _hotelCollection.Where(predicate);
    }
    
    public sealed class HotelCsvMap : ClassMap<Hotel>
    {
        private static readonly Regex SWhitespace = new(@"\s+");
        public HotelCsvMap()
        {
            Map(m => m.Id).Name("HOTELID");
            Map(m => m.City).Name("CITY");
            Map(m => m.RoomType).Name("ROOM").Convert(r =>
            {
                TryParse(ReplaceWhitespace(r.Row.GetField("ROOM").ToString()), out RoomType roomType);
                return roomType;
            });
            Map(m => m.Price).Name("PRICE");
        }
        
        public static string ReplaceWhitespace(string input) 
        {
            return SWhitespace.Replace(input, "");
        }
    }
}