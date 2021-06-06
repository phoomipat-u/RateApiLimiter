using System.ComponentModel;

namespace RateApiLimiter.Utilities
{
    public static class ControllerUtility
    {
        public const string Ascending = "ASC";
        public const string Descending = "DESC";
        
        public static ListSortDirection? GetSortDirection(string? direction)
        => direction switch
        {
            Ascending => ListSortDirection.Ascending,
            Descending => ListSortDirection.Descending,
            _ => null
        };
    }
}