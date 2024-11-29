using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeanScene.Data;
using BeanScene.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using BeanScene.Controllers.Api;

namespace BeanScene.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly BeanSceneContext _context;
        private readonly ILogger<ReservationsController> _logger;

        public ReservationsController(BeanSceneContext context, ILogger<ReservationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/reservations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
        {
            try
            {
                var reservations = await _context.Reservations
                    .Include(r => r.Guest)
                    .Include(r => r.Sitting)
                    .ToListAsync();

                return Ok(reservations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reservations");
                return StatusCode(500, new { message = "Error retrieving reservations", error = ex.Message });
            }
        }

        // GET: api/reservations/date/2024-03-20
        [HttpGet("date/{date}")]
        public async Task<ActionResult<IEnumerable<object>>> GetReservationsByDate(DateTime date)
        {
            try
            {
                var sydneyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Australia/Sydney");
                var sydneyDate = TimeZoneInfo.ConvertTimeFromUtc(date.ToUniversalTime(), sydneyTimeZone);
                var startOfDay = sydneyDate.Date;
                var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

                var reservations = await _context.Reservations
                    .Include(r => r.Guest)
                    .Include(r => r.Sitting)
                    .Include(r => r.Tables) 
                    .Where(r => r.StartTime >= startOfDay && r.StartTime <= endOfDay)
                    .Select(r => new
                    {
                        id = r.ReservationID,
                        start = r.StartTime,
                        name = $"{r.Guest.FirstName} {r.Guest.LastName}".Trim(),
                        phone = r.Guest.PhoneNumber,
                        sitting = r.Sitting.SittingType,
                        numberOfGuests = r.NumberOfGuests,
                        status = r.ReservationStatus,
                        tables = r.Tables.Select(t => t.TableID).ToList()
                    })
                    .ToListAsync();

                if (!reservations.Any())
                {
                    return NotFound(new { message = "No reservations found for this date" });
                }

                return Ok(reservations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reservations for date");
                return StatusCode(500, new { message = "Error retrieving reservations", error = ex.Message });
            }
        }

        // POST: api/reservations
        [HttpPost]
        public async Task<ActionResult<Reservation>> CreateReservation([FromBody] ReservationCreateDto dto)
        {
            try
            {
                //Populate the sittingID based on StartTime.
                var sitting = await _context.Sittings.FindAsync(dto.SittingID);
                // Check if the sitting exists
                if (sitting == null)
                {
                    return NotFound(new { message = "No Valid Sittings for Entered Start Time and Date" });
                }
                //Validate table
                var table = await _context.Tables.FindAsync(dto.TableID);
                if (table == null)
                {
                    return NotFound(new { message = "Table not found" });
                }

                // Find or create the guest
                var guest = await _context.Guests
                    .FirstOrDefaultAsync(g => g.PhoneNumber == dto.PhoneNumber);

                if (guest == null)
                {
                    guest = new Guest
                    {
                        FirstName = dto.FirstName,
                        LastName = dto.LastName,
                        PhoneNumber = dto.PhoneNumber,
                        Email = dto.Email
                    };
                    _context.Guests.Add(guest);
                    await _context.SaveChangesAsync();
                }

                // Create the reservation
                var reservation = new Reservation
                {
                    GuestID = guest.GuestID,
                    SittingID = dto.SittingID,
                    StartTime = dto.StartTime,
                    EndTime = dto.StartTime.AddMinutes(90), // Default 90-minute duration
                    NumberOfGuests = dto.NumberOfGuests,
                    ReservationStatus = "Pending",
                    Notes = dto.Notes
                };
                reservation.Tables.Add(table);

                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();

                //Map this reservation to ReservationResponseDto
                var response = new ReservationResponseDto
                {
                    ReservationID = reservation.ReservationID,
                    StartTime = reservation.StartTime,
                    EndTime = reservation.EndTime,
                    NumberOfGuests = reservation.NumberOfGuests,
                    ReservationStatus = reservation.ReservationStatus,
                    Notes = reservation.Notes!,
                    SittingID = reservation.SittingID,
                    Guest = new GuestDto
                    {
                        FirstName = dto.FirstName,
                        LastName = dto.LastName,
                        PhoneNumber = dto.PhoneNumber,
                        Email = dto.Email,
                    },
                    TableIDs = reservation.Tables.Select(t => t.TableID).ToList()
                };
                return CreatedAtAction(nameof(GetReservations), new { id = reservation.ReservationID }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating reservation");
                return StatusCode(500, new { message = "Error creating reservation", error = ex.Message });
            }
        }

        // PUT: api/reservations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReservation(int id, [FromBody] ReservationUpdateDto dto)
        {
            try
            {
                _logger.LogInformation($"Updating reservation {id} with data: {JsonSerializer.Serialize(dto)}");
                
                var reservation = await _context.Reservations
                    .Include(r => r.Guest)
                    .FirstOrDefaultAsync(r => r.ReservationID == id);

                if (reservation == null)
                {
                    return NotFound(new { message = "Reservation not found" });
                }

                // Update reservation details
                if (dto.StartTime != default)
                {
                    var sydneyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Australia/Sydney");
                    var sydneyTime = DateTime.SpecifyKind(dto.StartTime, DateTimeKind.Utc);
                    
                    reservation.StartTime = sydneyTime;
                    reservation.EndTime = sydneyTime.AddMinutes(90);
                    
                    _logger.LogInformation($"Updated time: Original={dto.StartTime}, Sydney={sydneyTime}");
                }
                
                if (dto.NumberOfGuests > 0)
                {
                    reservation.NumberOfGuests = dto.NumberOfGuests;
                }

                if (!string.IsNullOrEmpty(dto.ReservationStatus))
                {
                    reservation.ReservationStatus = dto.ReservationStatus;
                }

                if (dto.Notes != null)
                {
                    reservation.Notes = dto.Notes;
                }

                // Update guest details if provided
                if (reservation.Guest != null)
                {
                    if (!string.IsNullOrEmpty(dto.FirstName))
                        reservation.Guest.FirstName = dto.FirstName;
                    if (!string.IsNullOrEmpty(dto.LastName))
                        reservation.Guest.LastName = dto.LastName;
                    if (!string.IsNullOrEmpty(dto.PhoneNumber))
                        reservation.Guest.PhoneNumber = dto.PhoneNumber;
                    if (!string.IsNullOrEmpty(dto.Email))
                        reservation.Guest.Email = dto.Email;
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "Reservation updated successfully", reservation = new ReservationResponseDto {
                    ReservationID = reservation.ReservationID,
                    StartTime = reservation.StartTime,
                    EndTime = reservation.EndTime,
                    NumberOfGuests = reservation.NumberOfGuests,
                    ReservationStatus = reservation.ReservationStatus,
                    Notes = reservation.Notes!,
                    Guest = new GuestDto
                    {
                        FirstName = reservation.Guest!.FirstName,
                        LastName = reservation.Guest.LastName,
                        PhoneNumber = reservation.Guest.PhoneNumber,
                        Email = reservation.Guest.Email
                    }
                }});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating reservation {id}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // DELETE: api/reservations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            try
            {
                var reservation = await _context.Reservations.FindAsync(id);
                if (reservation == null)
                {
                    return NotFound(new { message = "Reservation not found" });
                }

                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Reservation successfully deleted" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting reservation");
                return StatusCode(500, new { message = "Error deleting reservation", error = ex.Message });
            }
        }
    }



    public class ReservationCreateDto
    {
        [Required]
        public int SittingID { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int NumberOfGuests { get; set; }

        [Required]
        public string FirstName { get; set; } = default!;

        [Required]
        public string LastName { get; set; } = default!;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = default!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        public string Notes { get; set; } = default!;
        [Required]
        public string TableID { get; set; } = default!;
        //DO NOT WORRY ABOUT BELOW, WAS TRYING TO MAKE A FUNCTION TO UPDATE SITTING ID BASED ON DATE AND TIME PASSED FROM FRONTEND
        ////New method that Should populate the SittingID based on the passed through time and date if it exists in the DB
        //public async Task<int?> PopulateSittingIdAsync(DbContext context)
        //{
        //    try
        //    {
        //        //Logging the StartTime for debugging purposes (Was having issues with the SittingID being null
        //        Console.WriteLine($"Populating SittingID for StartTime: {StartTime}");
        //        //Query backend DB
        //        var sitting = await context.Set<Sitting>()
        //            .FirstOrDefaultAsync(s => s.StartTime <= StartTime &&
        //                                      s.EndTime >= StartTime &&
        //                                      !s.ClosedForReservations);

        //        // Log the result of the sitting query
        //        if (sitting != null)
        //        {
        //            Console.WriteLine($"Found SittingID: {sitting.SittingID}");
        //            return sitting.SittingID;
        //        }
        //        else
        //        {
        //            Console.WriteLine("No valid sitting found.");
        //            return null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error in PopulateSittingIdAsync: {ex.Message}");
        //        throw; // Re-throw the exception to propagate it to the controller
        //    }
        //}
    }

    public class ReservationUpdateDto
    {
        public int ReservationID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int NumberOfGuests { get; set; }
        public string ReservationStatus { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Notes { get; set; } = default!;
        public int SittingID { get; set; }

    }

    public class ReservationResponseDto
    {
        public int ReservationID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int NumberOfGuests { get; set; }
        public string ReservationStatus { get; set; } = default!;
        public string Notes { get; set; } = default!;
        public GuestDto Guest { get; set; } = default!;
        public int SittingID { get; set; }
        public List<string> TableIDs { get; set; } = new();
    }

    public class GuestDto
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string Email { get; set; } = default!;
    }
}