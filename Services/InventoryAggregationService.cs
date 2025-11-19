using Microsoft.EntityFrameworkCore;
using NewLook.Data;
using NewLook.Models.DTOs.Api;
using NewLook.Services.Interfaces;

namespace NewLook.Services;

public class InventoryAggregationService : IInventoryAggregationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<InventoryAggregationService> _logger;

    public InventoryAggregationService(
        ApplicationDbContext context,
        ILogger<InventoryAggregationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<InventoryAggregatedDataDto> GetAggregatedDataAsync(int inventoryId)
    {
        var inventory = await _context.Inventories.FindAsync(inventoryId);
        if (inventory == null)
        {
            throw new Exception("Inventory not found");
        }

        var items = await _context.Items
            .Where(i => i.InventoryId == inventoryId)
            .ToListAsync();

        var result = new InventoryAggregatedDataDto
        {
            InventoryId = inventory.Id,
            Title = inventory.Title,
            Description = inventory.Description,
            TotalItems = items.Count,
            Fields = new List<FieldAggregationDto>()
        };

        AddStringFieldsAggregation(result, inventory, items);
        AddTextFieldsAggregation(result, inventory, items);
        AddNumberFieldsAggregation(result, inventory, items);
        AddLinkFieldsAggregation(result, inventory, items);
        AddBoolFieldsAggregation(result, inventory, items);

        return result;
    }

    private void AddStringFieldsAggregation(InventoryAggregatedDataDto result, Models.Entities.Inventory inventory, List<Models.Entities.Item> items)
    {
        if (inventory.CustomString1Enabled)
        {
            result.Fields.Add(new FieldAggregationDto
            {
                FieldTitle = inventory.CustomString1Name ?? "String 1",
                FieldType = "string",
                TextData = AggregateTextValues(items.Select(i => i.CustomString1Value).Where(v => !string.IsNullOrWhiteSpace(v)).ToList())
            });
        }

        if (inventory.CustomString2Enabled)
        {
            result.Fields.Add(new FieldAggregationDto
            {
                FieldTitle = inventory.CustomString2Name ?? "String 2",
                FieldType = "string",
                TextData = AggregateTextValues(items.Select(i => i.CustomString2Value).Where(v => !string.IsNullOrWhiteSpace(v)).ToList())
            });
        }

        if (inventory.CustomString3Enabled)
        {
            result.Fields.Add(new FieldAggregationDto
            {
                FieldTitle = inventory.CustomString3Name ?? "String 3",
                FieldType = "string",
                TextData = AggregateTextValues(items.Select(i => i.CustomString3Value).Where(v => !string.IsNullOrWhiteSpace(v)).ToList())
            });
        }
    }

    private void AddTextFieldsAggregation(InventoryAggregatedDataDto result, Models.Entities.Inventory inventory, List<Models.Entities.Item> items)
    {
        if (inventory.CustomText1Enabled)
        {
            result.Fields.Add(new FieldAggregationDto
            {
                FieldTitle = inventory.CustomText1Name ?? "Text 1",
                FieldType = "text",
                TextData = AggregateTextValues(items.Select(i => i.CustomText1Value).Where(v => !string.IsNullOrWhiteSpace(v)).ToList())
            });
        }

        if (inventory.CustomText2Enabled)
        {
            result.Fields.Add(new FieldAggregationDto
            {
                FieldTitle = inventory.CustomText2Name ?? "Text 2",
                FieldType = "text",
                TextData = AggregateTextValues(items.Select(i => i.CustomText2Value).Where(v => !string.IsNullOrWhiteSpace(v)).ToList())
            });
        }

        if (inventory.CustomText3Enabled)
        {
            result.Fields.Add(new FieldAggregationDto
            {
                FieldTitle = inventory.CustomText3Name ?? "Text 3",
                FieldType = "text",
                TextData = AggregateTextValues(items.Select(i => i.CustomText3Value).Where(v => !string.IsNullOrWhiteSpace(v)).ToList())
            });
        }
    }

    private void AddNumberFieldsAggregation(InventoryAggregatedDataDto result, Models.Entities.Inventory inventory, List<Models.Entities.Item> items)
    {
        if (inventory.CustomNumber1Enabled)
        {
            result.Fields.Add(new FieldAggregationDto
            {
                FieldTitle = inventory.CustomNumber1Name ?? "Number 1",
                FieldType = "number",
                NumberData = AggregateNumberValues(items.Select(i => i.CustomNumber1Value).Where(v => v.HasValue).Select(v => v!.Value).ToList())
            });
        }

        if (inventory.CustomNumber2Enabled)
        {
            result.Fields.Add(new FieldAggregationDto
            {
                FieldTitle = inventory.CustomNumber2Name ?? "Number 2",
                FieldType = "number",
                NumberData = AggregateNumberValues(items.Select(i => i.CustomNumber2Value).Where(v => v.HasValue).Select(v => v!.Value).ToList())
            });
        }

        if (inventory.CustomNumber3Enabled)
        {
            result.Fields.Add(new FieldAggregationDto
            {
                FieldTitle = inventory.CustomNumber3Name ?? "Number 3",
                FieldType = "number",
                NumberData = AggregateNumberValues(items.Select(i => i.CustomNumber3Value).Where(v => v.HasValue).Select(v => v!.Value).ToList())
            });
        }
    }

    private void AddLinkFieldsAggregation(InventoryAggregatedDataDto result, Models.Entities.Inventory inventory, List<Models.Entities.Item> items)
    {
        if (inventory.CustomLink1Enabled)
        {
            result.Fields.Add(new FieldAggregationDto
            {
                FieldTitle = inventory.CustomLink1Name ?? "Link 1",
                FieldType = "link",
                TextData = AggregateTextValues(items.Select(i => i.CustomLink1Value).Where(v => !string.IsNullOrWhiteSpace(v)).ToList())
            });
        }

        if (inventory.CustomLink2Enabled)
        {
            result.Fields.Add(new FieldAggregationDto
            {
                FieldTitle = inventory.CustomLink2Name ?? "Link 2",
                FieldType = "link",
                TextData = AggregateTextValues(items.Select(i => i.CustomLink2Value).Where(v => !string.IsNullOrWhiteSpace(v)).ToList())
            });
        }

        if (inventory.CustomLink3Enabled)
        {
            result.Fields.Add(new FieldAggregationDto
            {
                FieldTitle = inventory.CustomLink3Name ?? "Link 3",
                FieldType = "link",
                TextData = AggregateTextValues(items.Select(i => i.CustomLink3Value).Where(v => !string.IsNullOrWhiteSpace(v)).ToList())
            });
        }
    }

    private void AddBoolFieldsAggregation(InventoryAggregatedDataDto result, Models.Entities.Inventory inventory, List<Models.Entities.Item> items)
    {
        if (inventory.CustomBool1Enabled)
        {
            var trueCount = items.Count(i => i.CustomBool1Value == true);
            var falseCount = items.Count(i => i.CustomBool1Value == false);
            result.Fields.Add(new FieldAggregationDto
            {
                FieldTitle = inventory.CustomBool1Name ?? "Boolean 1",
                FieldType = "boolean",
                TextData = new TextAggregation
                {
                    MostPopularValues = new List<PopularValueDto>
                    {
                        new() { Value = "True", Count = trueCount },
                        new() { Value = "False", Count = falseCount }
                    },
                    TotalUniqueValues = 2
                }
            });
        }

        if (inventory.CustomBool2Enabled)
        {
            var trueCount = items.Count(i => i.CustomBool2Value == true);
            var falseCount = items.Count(i => i.CustomBool2Value == false);
            result.Fields.Add(new FieldAggregationDto
            {
                FieldTitle = inventory.CustomBool2Name ?? "Boolean 2",
                FieldType = "boolean",
                TextData = new TextAggregation
                {
                    MostPopularValues = new List<PopularValueDto>
                    {
                        new() { Value = "True", Count = trueCount },
                        new() { Value = "False", Count = falseCount }
                    },
                    TotalUniqueValues = 2
                }
            });
        }

        if (inventory.CustomBool3Enabled)
        {
            var trueCount = items.Count(i => i.CustomBool3Value == true);
            var falseCount = items.Count(i => i.CustomBool3Value == false);
            result.Fields.Add(new FieldAggregationDto
            {
                FieldTitle = inventory.CustomBool3Name ?? "Boolean 3",
                FieldType = "boolean",
                TextData = new TextAggregation
                {
                    MostPopularValues = new List<PopularValueDto>
                    {
                        new() { Value = "True", Count = trueCount },
                        new() { Value = "False", Count = falseCount }
                    },
                    TotalUniqueValues = 2
                }
            });
        }
    }

    private NumberAggregation AggregateNumberValues(List<decimal> values)
    {
        if (!values.Any())
        {
            return new NumberAggregation { Count = 0 };
        }

        return new NumberAggregation
        {
            Average = values.Average(),
            Min = values.Min(),
            Max = values.Max(),
            Count = values.Count
        };
    }

    private TextAggregation AggregateTextValues(List<string?> values)
    {
        var valueCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var value in values.Where(v => !string.IsNullOrWhiteSpace(v)))
        {
            if (valueCounts.ContainsKey(value!))
            {
                valueCounts[value!]++;
            }
            else
            {
                valueCounts[value!] = 1;
            }
        }

        var mostPopular = valueCounts
            .OrderByDescending(kvp => kvp.Value)
            .Take(10)
            .Select(kvp => new PopularValueDto
            {
                Value = kvp.Key,
                Count = kvp.Value
            })
            .ToList();

        return new TextAggregation
        {
            MostPopularValues = mostPopular,
            TotalUniqueValues = valueCounts.Count
        };
    }
}
