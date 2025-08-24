namespace BusinessLogic.Entities
{
    public class CustomCakeOption
    {
        public int OptionId { get; set; }
        public string OptionType { get; set; } = null!; // Size/Flavor/Decoration
        public string Name { get; set; } = null!;
        public decimal ExtraPrice { get; set; } = 0m;
    }

}
