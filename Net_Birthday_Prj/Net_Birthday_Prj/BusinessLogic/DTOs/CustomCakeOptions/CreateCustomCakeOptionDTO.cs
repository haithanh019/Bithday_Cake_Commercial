namespace BusinessLogic.DTOs.CustomCakeOptions
{
    public class CreateCustomCakeOptionDTO
    {
        public string OptionType { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal ExtraPrice { get; set; } = 0m;
    }
}
