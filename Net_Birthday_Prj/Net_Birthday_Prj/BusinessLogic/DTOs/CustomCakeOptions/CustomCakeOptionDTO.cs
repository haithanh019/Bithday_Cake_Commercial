using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.DTOs.CustomCakeOptions
{
    public class CustomCakeOptionDTO
    {
        [Key]
        public int OptionId { get; set; }
        public string OptionType { get; set; } = null!;   // Size / Flavor / Decoration
        public string Name { get; set; } = null!;
        public decimal ExtraPrice { get; set; }
    }
}

