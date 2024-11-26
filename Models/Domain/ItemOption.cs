using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeanScene.Models {

    public class ItemOption
    {
        [Key]
        public int OptionID { get; set; }

        [Required]
        public int ItemID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = default!;

        [StringLength(200)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal PriceModifier { get; set; }

        // Navigation properties
        public MenuItem MenuItem { get; set; } = default!;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

}