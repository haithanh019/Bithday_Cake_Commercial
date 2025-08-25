using BusinessLogic.Enums;

namespace BusinessLogic.Entities
{
    public class CustomCakeOption
    {
        public int OptionId { get; set; }
        public string Name { get; set; } = null!;
        public decimal ExtraPrice { get; set; } = 0m;
        public CustomOptionType OptionType { get; set; }
    }

}
