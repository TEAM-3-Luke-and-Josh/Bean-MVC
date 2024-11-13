using System.ComponentModel.DataAnnotations;

namespace BeanScene.Models {

    public class Sitting
    {
        [Key]
        public int SittingID { get; set; }

        [Required]
        [Display(Name = "Sitting Type")]
        public string SittingType { get; set; } = default!;

        [Required]
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        [Required]
        [Display(Name = "End Time")]
        public DateTime EndTime { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0")]
        public int Capacity { get; set; }

        [Display(Name = "Closed For Reservations")]
        public bool ClosedForReservations { get; set; } = false;

        // Navigation property
        public ICollection<Reservation> Reservations { get; set; } = default!;
    }

}