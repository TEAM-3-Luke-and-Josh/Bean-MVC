using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeanScene.Models {

    public class OrderItem
    {
        [Key]
        public int OrderItemID { get; set; }

        [Required]
        public int OrderID { get; set; }

        [Required]
        public int ItemID { get; set; }

        [Required]
        public int Quantity { get; set; } = 1;

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Subtotal { get; set; }

        [StringLength(200)]
        public string? SpecialInstructions { get; set; }

        [Required]
        [StringLength(50)]
        public string ItemStatus { get; set; } = "Pending";

        // Navigation properties
        public Order Order { get; set; } = default!;
        public MenuItem MenuItem { get; set; } = default!;
        public ICollection<ItemOption> SelectedOptions { get; set; } = new List<ItemOption>();
    }

}