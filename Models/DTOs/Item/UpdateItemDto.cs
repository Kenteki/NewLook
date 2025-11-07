namespace NewLook.Models.DTOs.Item
{
    public class UpdateItemDto
    {
        public string? CustomId { get; set; }
        
        public string? CustomString1Value { get; set; }
        public string? CustomString2Value { get; set; }
        public string? CustomString3Value { get; set; }
        
        public string? CustomText1Value { get; set; }
        public string? CustomText2Value { get; set; }
        public string? CustomText3Value { get; set; }
        
        public decimal? CustomNumber1Value { get; set; }
        public decimal? CustomNumber2Value { get; set; }
        public decimal? CustomNumber3Value { get; set; }
        
        public string? CustomLink1Value { get; set; }
        public string? CustomLink2Value { get; set; }
        public string? CustomLink3Value { get; set; }
        
        public bool? CustomBool1Value { get; set; }
        public bool? CustomBool2Value { get; set; }
        public bool? CustomBool3Value { get; set; }
        
        public int Version { get; set; } // Version control
    }
}