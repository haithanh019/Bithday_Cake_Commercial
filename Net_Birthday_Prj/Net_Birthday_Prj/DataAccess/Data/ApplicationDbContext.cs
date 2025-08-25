using BusinessLogic.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<
        ApplicationUser, IdentityRole<int>, int,
        IdentityUserClaim<int>, IdentityUserRole<int>, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<CustomCakeOption> CustomCakeOptions => Set<CustomCakeOption>();
        public DbSet<ShoppingCart> ShoppingCarts => Set<ShoppingCart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();

        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();



        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // ===== Categories =====
            b.Entity<Category>(e =>
            {
                e.HasKey(x => x.CategoryId);
                e.Property(x => x.Name).IsRequired().HasMaxLength(100);
                e.Property(x => x.Description).HasMaxLength(300);
            });

            // ===== Products =====
            b.Entity<Product>(e =>
            {
                e.HasKey(x => x.ProductId);
                e.Property(x => x.Name).IsRequired().HasMaxLength(150);
                e.Property(x => x.Description).HasMaxLength(500);
                e.Property(x => x.ImageUrl).HasMaxLength(250);
                e.Property(x => x.Price).HasColumnType("decimal(10,2)");
                e.HasOne(x => x.Category)
                 .WithMany(c => c.Products)
                 .HasForeignKey(x => x.CategoryId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== CustomCakeOptions =====
            b.Entity<CustomCakeOption>(e =>
            {
                e.HasKey(x => x.OptionId);
                e.Property(x => x.OptionType).IsRequired().HasMaxLength(50);
                e.Property(x => x.Name).IsRequired().HasMaxLength(100);
                e.Property(x => x.ExtraPrice).HasColumnType("decimal(10,2)");
            });

            // ===== ShoppingCarts =====
            b.Entity<ShoppingCart>(e =>
            {
                e.HasKey(x => x.CartId);
                e.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
                e.HasOne(x => x.User)
                 .WithMany(u => u.ShoppingCarts)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ===== CartItem =====
            b.Entity<CartItem>(e =>
            {
                e.HasKey(x => x.CartItemId);

                e.Property(x => x.Quantity)
                    .IsRequired()
                    .HasDefaultValue(1);

                e.Property(x => x.UnitPrice)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                e.HasOne(x => x.Cart)
                    .WithMany(c => c.Items)
                    .HasForeignKey(x => x.CartId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Product)
                    .WithMany() // nếu Product chưa có ICollection<CartItem>, để trống
                    .HasForeignKey(x => x.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== Orders =====
            b.Entity<Order>(e =>
            {
                e.HasKey(x => x.OrderId);
                e.Property(x => x.TotalAmount).HasColumnType("decimal(10,2)");
                e.Property(x => x.Status).HasMaxLength(50).HasDefaultValue("Pending");
                e.Property(x => x.DeliveryAddress).HasMaxLength(250);
                e.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");

                e.HasOne(x => x.User)
                 .WithMany(u => u.Orders)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Cart)
                 .WithMany()
                 .HasForeignKey(x => x.CartId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== OrderItems =====
            b.Entity<OrderItem>(e =>
            {
                e.HasKey(x => x.OrderItemId);
                e.Property(x => x.Price).HasColumnType("decimal(10,2)");
                e.HasOne(x => x.Order)
                 .WithMany(o => o.Items)
                 .HasForeignKey(x => x.OrderId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Product)
                 .WithMany()
                 .HasForeignKey(x => x.ProductId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== Seed dữ liệu Domain (dùng HasData cần ID cố định) =====
            b.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Bánh sinh nhật", Description = "Bánh kem truyền thống và hiện đại cho sinh nhật" },
                new Category { CategoryId = 2, Name = "Cupcake", Description = "Bánh cupcake nhỏ gọn nhiều hương vị" },
                new Category { CategoryId = 3, Name = "Custom", Description = "Thiết kế bánh theo ý muốn khách hàng" }
            );

            b.Entity<Product>().HasData(
                new Product { ProductId = 1, CategoryId = 1, Name = "Bánh Kem Socola", Description = "Bánh kem socola phủ kem tươi", Price = 250000m, ImageUrl = "/images/choco_cake.jpg", IsAvailable = true },
                new Product { ProductId = 2, CategoryId = 1, Name = "Bánh Kem Dâu", Description = "Bánh kem vani phủ dâu tươi", Price = 270000m, ImageUrl = "/images/strawberry_cake.jpg", IsAvailable = true }
            // Dòng (1, 3, 1, 30000) trong SQL bạn đưa bị sai cột -> bỏ hoặc sửa thành:
            // new Product { ProductId = 3, CategoryId = 2, Name = "Cupcake Vani", Description = "Cupcake vị vani", Price = 30000m, ImageUrl = "/images/vanilla_cupcake.jpg", IsAvailable = true }
            );
        }
    }
}
