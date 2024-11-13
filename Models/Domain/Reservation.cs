using System.ComponentModel.DataAnnotations;

namespace BeanScene.Models {

    public class Reservation
    {
        [Key]
        public int ReservationID { get; set; }

        [Required]
        public int GuestID { get; set; }

        [Required]
        public int SittingID { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        [Required]
        [Display(Name = "End Time")]
        public DateTime EndTime { get; set; }

        [Required]
        [Display(Name = "Number of Guests")]
        [Range(1, int.MaxValue, ErrorMessage = "Number of guests must be greater than 0")]
        public int NumberOfGuests { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string ReservationStatus { get; set; } = "Pending";

        public string? Notes { get; set; }

        // Navigation properties
        public Guest Guest { get; set; } = default!;
        public Sitting Sitting { get; set; } = default!;
        public ICollection<Table> Tables { get; set; } = new List<Table>();
    }
}
