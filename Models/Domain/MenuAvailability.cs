using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeanScene.Models {

    public class MenuAvailability
    {
        [Required]
        public int ItemID { get; set; }

        [Required]
        [StringLength(50)]
        public string SittingType { get; set; } = default!;

        public bool IsAvailable { get; set; } = true;

        // Navigation property
        public MenuItem MenuItem { get; set; } = default!;
    }

}