using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeanScene.Data;
using BeanScene.Models;
using System.ComponentModel.DataAnnotations;

namespace BeanScene.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestsController : ControllerBase
    {
        private readonly BeanSceneContext _context;
        private readonly ILogger<GuestsController> _logger;

        public GuestsController(BeanSceneContext context, ILogger<GuestsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/guests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Guest>>> GetGuests()
        {
            try
            {
                var guests = await _context.Guests
                    .Select(g => new
                    {
                        g.GuestID,
                        g.FirstName,
                        g.LastName,
                        g.Email,
                        g.PhoneNumber,
                        ReservationsCount = g.Reservations.Count
                    })
                    .ToListAsync();

                return Ok(guests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving guests");
                return StatusCode(500, new { message = "Error retrieving guests", error = ex.Message });
            }
        }

        // GET: api/guests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Guest>> GetGuest(int id)
        {
            try
            {
                var guest = await _context.Guests
                    .Include(g => g.Reservations)
                    .FirstOrDefaultAsync(g => g.GuestID == id);

                if (guest == null)
                {
                    return NotFound(new { message = "Guest not found" });
                }

                return Ok(guest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving guest");
                return StatusCode(500, new { message = "Error retrieving guest", error = ex.Message });
            }
        }

        // POST: api/guests
        [HttpPost]
        public async Task<ActionResult<Guest>> CreateGuest(GuestCreateDto dto)
        {
            try
            {
                // Check if guest with same email or phone already exists
                var existingGuest = await _context.Guests
                    .FirstOrDefaultAsync(g => g.Email == dto.Email || g.PhoneNumber == dto.PhoneNumber);

                if (existingGuest != null)
                {
                    return Conflict(new { message = "Guest with this email or phone number already exists" });
                }

                var guest = new Guest
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber
                };

                _context.Guests.Add(guest);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetGuest), new { id = guest.GuestID }, guest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating guest");
                return StatusCode(500, new { message = "Error creating guest", error = ex.Message });
            }
        }

        // PUT: api/guests/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGuest(int id, GuestUpdateDto dto)
        {
            try
            {
                var guest = await _context.Guests.FindAsync(id);
                if (guest == null)
                {
                    return NotFound(new { message = "Guest not found" });
                }

                // Check if email/phone updates would conflict with existing guests
                if (dto.Email != guest.Email || dto.PhoneNumber != guest.PhoneNumber)
                {
                    var existingGuest = await _context.Guests
                        .FirstOrDefaultAsync(g => g.GuestID != id && 
                            (g.Email == dto.Email || g.PhoneNumber == dto.PhoneNumber));

                    if (existingGuest != null)
                    {
                        return Conflict(new { message = "Another guest with this email or phone number already exists" });
                    }
                }

                guest.FirstName = dto.FirstName ?? guest.FirstName;
                guest.LastName = dto.LastName ?? guest.LastName;
                guest.Email = dto.Email ?? guest.Email;
                guest.PhoneNumber = dto.PhoneNumber ?? guest.PhoneNumber;

                await _context.SaveChangesAsync();
                return Ok(new { message = "Guest updated successfully", guest });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating guest");
                return StatusCode(500, new { message = "Error updating guest", error = ex.Message });
            }
        }

        // DELETE: api/guests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGuest(int id)
        {
            try
            {
                var guest = await _context.Guests
                    .Include(g => g.Reservations)
                    .FirstOrDefaultAsync(g => g.GuestID == id);

                if (guest == null)
                {
                    return NotFound(new { message = "Guest not found" });
                }

                // Check if guest has any reservations
                if (guest.Reservations.Any())
                {
                    return Conflict(new { message = "Cannot delete guest with existing reservations" });
                }

                _context.Guests.Remove(guest);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Guest successfully deleted" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting guest");
                return StatusCode(500, new { message = "Error deleting guest", error = ex.Message });
            }
        }
    }

    public class GuestCreateDto
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = default!;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = default!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = default!;
    }

    public class GuestUpdateDto
    {
        [StringLength(50)]
        public string FirstName { get; set; } = default!;

        [StringLength(50)]
        public string LastName { get; set; } = default!;

        [EmailAddress]
        public string Email { get; set; } = default!;

        [Phone]
        public string PhoneNumber { get; set; } = default!;
    }
}