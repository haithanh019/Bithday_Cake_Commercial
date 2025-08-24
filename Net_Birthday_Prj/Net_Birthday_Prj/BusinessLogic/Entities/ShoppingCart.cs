namespace BusinessLogic.Entities
{
    public class ShoppingCart
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public ApplicationUser User { get; set; } = null!;
    }

}
