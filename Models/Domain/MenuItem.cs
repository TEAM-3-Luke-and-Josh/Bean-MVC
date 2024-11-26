using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeanScene.Models {

    public class MenuItem
    {
        [Key]
        public int ItemID { get; set; }

        [Required]
        public int CategoryID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = default!;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }

        public bool IsAvailable { get; set; } = true;

        public int? PrepTime { get; set; }

        [StringLength(255)]
        public string? ImageUrl { get; set; }

        // Navigation properties
        public MenuCategory Category { get; set; } = default!;
        public ICollection<ItemOption> Options { get; set; } = new List<ItemOption>();
        public ICollection<MenuAvailability> Availability { get; set; } = new List<MenuAvailability>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

}