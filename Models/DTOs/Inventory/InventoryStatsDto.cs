namespace NewLook.Models.DTOs.Inventory
{
    public class InventoryStatsDto
    {
        public int TotalItems { get; set; }
        public Dictionary<string, object> FieldStats { get; set; } = new();
    }

    public class NumericFieldStats
    {
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public decimal Average { get; set; }
        public int Count { get; set; }
    }

    public class StringFieldStats
    {
        public List<ValueCount> TopValues { get; set; } = new();
        public int UniqueCount { get; set; }
    }

    public class ValueCount
    {
        public string Value { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}