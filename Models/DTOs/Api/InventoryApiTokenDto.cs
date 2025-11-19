namespace NewLook.Models.DTOs.Api;

public class InventoryApiTokenDto
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public string? Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastUsedAt { get; set; }
}

public class CreateApiTokenDto
{
    public string? Name { get; set; }
    public int? ExpiresInDays { get; set; }
}

public class InventoryAggregatedDataDto
{
    public int InventoryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<FieldAggregationDto> Fields { get; set; } = new();
    public int TotalItems { get; set; }
}

public class FieldAggregationDto
{
    public string FieldTitle { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty;
    public NumberAggregation? NumberData { get; set; }
    public TextAggregation? TextData { get; set; }
}

public class NumberAggregation
{
    public decimal? Average { get; set; }
    public decimal? Min { get; set; }
    public decimal? Max { get; set; }
    public int Count { get; set; }
}

public class TextAggregation
{
    public List<PopularValueDto> MostPopularValues { get; set; } = new();
    public int TotalUniqueValues { get; set; }
}

public class PopularValueDto
{
    public string Value { get; set; } = string.Empty;
    public int Count { get; set; }
}
