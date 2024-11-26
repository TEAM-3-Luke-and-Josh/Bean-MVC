using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeanScene.Models {

    public class Order
    {
        [Key]
        public int OrderID { get; set; }

        public int? ReservationID { get; set; }

        [StringLength(4)]
        public string? TableID { get; set; }

        [Required]
        [StringLength(50)]
        public string OrderStatus { get; set; } = "Pending";

        [Required]
        public DateTime OrderTime { get; set; } = DateTime.Now;

        [StringLength(500)]
        public string? SpecialRequests { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalAmount { get; set; }

        // Navigation properties
        public Reservation? Reservation { get; set; }
        public Table? Table { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<Table> AssignedTables { get; set; } = new List<Table>();
    }

}