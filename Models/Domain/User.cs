
using System.ComponentModel.DataAnnotations;

namespace BeanScene.Models {
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 5)]
        public string Username { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        [Display(Name = "User Type")]
        public string UserType { get; set; } = default!;
    }
}