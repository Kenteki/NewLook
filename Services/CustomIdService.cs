using Microsoft.EntityFrameworkCore;
using NewLook.Data;
using NewLook.Models.Entities;
using NewLook.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace NewLook.Services
{
    public class CustomIdService : ICustomIdService
    {
        private readonly ApplicationDbContext _context;
        private readonly Random _random = new Random();

        public CustomIdService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Generates a custom ID for a new item based on inventory's custom ID configuration
        /// </summary>
        public async Task<string> GenerateCustomIdAsync(int inventoryId)
        {
            var elements = await _context.CustomIdElements
                .Where(e => e.InventoryId == inventoryId)
                .OrderBy(e => e.Order)
                .ToListAsync();

            if (!elements.Any())
            {
                // No custom ID format defined, generate a simple sequential ID
                var maxSequence = await _context.Items
                    .Where(i => i.InventoryId == inventoryId)
                    .CountAsync();
                return $"ITEM-{(maxSequence + 1):D6}";
            }

            var sb = new StringBuilder();
            var createdAt = DateTime.UtcNow;

            foreach (var element in elements)
            {
                var part = await GenerateElementValueAsync(element, inventoryId, createdAt);
                sb.Append(part);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generates a preview of what the custom ID will look like
        /// Used in the UI when user is configuring the format
        /// </summary>
        public Task<string> PreviewCustomIdAsync(List<CustomIdElementDto> elements)
        {
            var sb = new StringBuilder();
            var previewDate = DateTime.UtcNow;
            var previewSequence = 1;

            foreach (var element in elements.OrderBy(e => e.Order))
            {
                var part = GeneratePreviewValue(element, previewDate, previewSequence);
                sb.Append(part);
            }

            return Task.FromResult(sb.ToString());
        }

        /// <summary>
        /// Validates if a custom ID matches the inventory's format rules
        /// </summary>
        public async Task<bool> ValidateCustomIdFormatAsync(int inventoryId, string customId)
        {
            var elements = await _context.CustomIdElements
                .Where(e => e.InventoryId == inventoryId)
                .OrderBy(e => e.Order)
                .ToListAsync();

            // If no format defined, any ID is valid
            if (!elements.Any()) return true;

            // Check if all fixed parts are present in correct positions
            var position = 0;
            foreach (var element in elements)
            {
                if (element.ElementType == "Fixed" && !string.IsNullOrEmpty(element.Value))
                {
                    if (!customId.Substring(position).StartsWith(element.Value))
                        return false;
                    position += element.Value.Length;
                }
            }

            return true;
        }

        private async Task<string> GenerateElementValueAsync(CustomIdElement element, int inventoryId, DateTime createdAt)
        {
            switch (element.ElementType)
            {
                case "Fixed":
                    return element.Value ?? "";

                case "Random20":
                    return GenerateRandom20Bit(element.Value);

                case "Random32":
                    return GenerateRandom32Bit(element.Value);

                case "Random6":
                    return _random.Next(0, 1000000).ToString("D6");

                case "Random9":
                    return _random.Next(0, 1000000000).ToString("D9");

                case "Guid":
                    return GenerateGuidValue(element.Value);

                case "DateTime":
                    return FormatDateTime(createdAt, element.Value);

                case "Sequence":
                    return await GenerateSequenceValueAsync(inventoryId, element.Value);

                default:
                    return "";
            }
        }

        private string GeneratePreviewValue(CustomIdElementDto element, DateTime previewDate, int previewSequence)
        {
            switch (element.ElementType)
            {
                case "Fixed":
                    return element.Value ?? "";

                case "Random20":
                    return "A7E3A"; // Example 20-bit hex (5 chars)

                case "Random32":
                    return "E74FA329"; // Example 32-bit hex (8 chars)

                case "Random6":
                    return "123456";

                case "Random9":
                    return "987654321";

                case "Guid":
                    return "550e8400-e29b-41d4-a716-446655440000";

                case "DateTime":
                    return FormatDateTime(previewDate, element.Value);

                case "Sequence":
                    var format = ParseFormatString(element.Value);
                    return previewSequence.ToString(format);

                default:
                    return "";
            }
        }

        private string GenerateRandom20Bit(string? formatString)
        {
            // 20-bit = 0 to 1,048,575 (0xFFFFF)
            var value = _random.Next(0, 1048576);
            var format = ParseFormatString(formatString);

            if (format.StartsWith("X"))
                return value.ToString(format); // Hex format
            else
                return value.ToString(format); // Decimal format
        }

        private string GenerateRandom32Bit(string? formatString)
        {
            // 32-bit = 0 to 4,294,967,295 (0xFFFFFFFF)
            var value = (uint)_random.Next() | ((uint)_random.Next() << 1);
            var format = ParseFormatString(formatString);

            if (format.StartsWith("X"))
                return value.ToString(format); // Hex format
            else
                return value.ToString(format); // Decimal format
        }

        private string GenerateGuidValue(string? formatString)
        {
            var guid = Guid.NewGuid();

            // Parse format: "N" (no dashes), "D" (default), "B" (braces), etc.
            if (string.IsNullOrEmpty(formatString))
                return guid.ToString("N").Substring(0, 8).ToUpper(); // Short version

            var options = JsonSerializer.Deserialize<GuidFormatOptions>(formatString);
            var guidStr = guid.ToString(options?.Format ?? "N");

            if (options?.Length > 0)
                guidStr = guidStr.Substring(0, Math.Min(options.Length, guidStr.Length));

            return options?.Uppercase == true ? guidStr.ToUpper() : guidStr.ToLower();
        }

        private string FormatDateTime(DateTime date, string? formatString)
        {
            if (string.IsNullOrEmpty(formatString))
                return date.ToString("yyyy-MM-dd");

            try
            {
                var options = JsonSerializer.Deserialize<DateTimeFormatOptions>(formatString);
                return date.ToString(options?.Format ?? "yyyy-MM-dd");
            }
            catch
            {
                return date.ToString("yyyy-MM-dd");
            }
        }

        private async Task<string> GenerateSequenceValueAsync(int inventoryId, string? formatString)
        {
            // Get the maximum sequence value for this inventory
            var maxSequence = await _context.Items
                .Where(i => i.InventoryId == inventoryId)
                .CountAsync();

            var nextSequence = maxSequence + 1;
            var format = ParseFormatString(formatString);

            return nextSequence.ToString(format);
        }

        private string ParseFormatString(string? formatString)
        {
            if (string.IsNullOrEmpty(formatString))
                return "D"; // Default format

            try
            {
                var options = JsonSerializer.Deserialize<FormatOptions>(formatString);
                return options?.Format ?? "D";
            }
            catch
            {
                return formatString; // Use as-is if not JSON
            }
        }
    }

    // DTOs for format options
    public class FormatOptions
    {
        public string Format { get; set; } = "D";
    }

    public class GuidFormatOptions
    {
        public string Format { get; set; } = "N"; // N, D, B, P, X
        public int Length { get; set; } = 8;
        public bool Uppercase { get; set; } = true;
    }

    public class DateTimeFormatOptions
    {
        public string Format { get; set; } = "yyyy-MM-dd";
    }

    // DTO for preview (used in UI)
    public class CustomIdElementDto
    {
        public int Order { get; set; }
        public string ElementType { get; set; } = string.Empty;
        public string? Value { get; set; }
    }
}