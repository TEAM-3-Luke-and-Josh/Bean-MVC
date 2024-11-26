using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeanScene.Models {

        public class MenuCategory
    {
        [Key]
        public int CategoryID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = default!;

        [StringLength(200)]
        public string? Description { get; set; }

        public bool IsAvailable { get; set; } = true;

        // Navigation property
        public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    }

}