using System.ComponentModel.DataAnnotations;

namespace BeanScene.Models {

    public class Guest
    {
        [Key]
        public int GuestID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = default!;

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = default!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = default!;

        // Navigation property
        public ICollection<Reservation> Reservations { get; set; } = default!;
    }
}