using System.ComponentModel.DataAnnotations;

namespace BeanScene.Models {
    public class Table
    {
        [Key]
        [StringLength(4, MinimumLength = 2)]
        public string TableID { get; set; } = default!;

        [Required]
        public string Area { get; set; } = default!;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0")]
        public int Capacity { get; set; }
    }
}
