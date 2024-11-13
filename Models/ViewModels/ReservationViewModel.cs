using System.ComponentModel.DataAnnotations;

namespace BeanScene.ViewModels
{
    public class ReservationViewModel
    {
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Reservation Date")]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "Sitting")]
        public int SittingId { get; set; }

        [Required]
        [Range(1, 20)]
        [Display(Name = "Number of Guests")]
        public int NumberOfGuests { get; set; }

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
        [Display(Name = "Email Address")]
        public string Email { get; set; } = default!;

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = default!;

        [Display(Name = "Special Requests")]
        public string Notes { get; set; } = default!;
    }
}
