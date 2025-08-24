using Microsoft.AspNetCore.Identity;
namespace BusinessLogic.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        // Identity đã có PhoneNumber, Email, PasswordHash sẵn.
        // Ta thêm FullName:
        public string FullName { get; set; } = string.Empty;

        // Quan hệ điều hướng
        public ICollection<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
