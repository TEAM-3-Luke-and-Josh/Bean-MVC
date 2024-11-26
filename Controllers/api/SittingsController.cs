using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BeanScene.Data;
using BeanScene.Models;
using System.ComponentModel.DataAnnotations;

namespace BeanScene.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class SittingsController : ControllerBase
    {
        private readonly BeanSceneContext _context;
        private readonly ILogger<SittingsController> _logger;

        public SittingsController(BeanSceneContext context, ILogger<SittingsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/sittings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sitting>>> GetSittings()
        {
            try
            {
                var sittings = await _context.Sittings
                    .OrderBy(s => s.StartTime)
                    .ToListAsync();
                return Ok(sittings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sittings");
                return StatusCode(500, new { message = "Error retrieving sittings", error = ex.Message });
            }
        }

        // GET: api/sittings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Sitting>> GetSitting(int id)
        {
            try
            {
                var sitting = await _context.Sittings
                    .Include(s => s.Reservations)
                    .FirstOrDefaultAsync(s => s.SittingID == id);

                if (sitting == null)
                {
                    return NotFound(new { message = "Sitting not found" });
                }

                return Ok(sitting);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sitting");
                return StatusCode(500, new { message = "Error retrieving sitting", error = ex.Message });
            }
        }

        // GET: api/sittings/available?date=2024-03-20
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<Sitting>>> GetAvailableSittings([FromQuery] DateTime? date)
        {
            try
            {
                var queryDate = date ?? DateTime.Today;
                var startOfDay = queryDate.Date;
                var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

                var sittings = await _context.Sittings
                    .Where(s => s.StartTime >= startOfDay && 
                               s.StartTime <= endOfDay &&
                               !s.ClosedForReservations)
                    .OrderBy(s => s.StartTime)
                    .ToListAsync();

                return Ok(sittings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available sittings");
                return StatusCode(500, new { message = "Error retrieving available sittings", error = ex.Message });
            }
        }

        // GET: api/sittings/date/2024-03-20
        [HttpGet("date/{date}")]
        public async Task<ActionResult<IEnumerable<Sitting>>> GetSittingsByDate(DateTime date)
        {
            try
            {
                var startOfDay = date.Date;
                var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

                var sittings = await _context.Sittings
                    .Where(s => s.StartTime >= startOfDay && s.StartTime <= endOfDay)
                    .OrderBy(s => s.StartTime)
                    .ToListAsync();

                if (!sittings.Any())
                {
                    return NotFound(new { message = "No sittings found for this date" });
                }

                return Ok(sittings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sittings by date");
                return StatusCode(500, new { message = "Error retrieving sittings", error = ex.Message });
            }
        }

        // POST: api/sittings
        [HttpPost]
        public async Task<ActionResult<Sitting>> CreateSitting(SittingCreateDto dto)
        {
            try
            {
                // Validate that EndTime is after StartTime
                if (dto.EndTime <= dto.StartTime)
                {
                    return BadRequest(new { message = "End time must be after start time" });
                }

                // Check for overlapping sittings
                var overlapping = await _context.Sittings
                    .AnyAsync(s => 
                        (dto.StartTime >= s.StartTime && dto.StartTime < s.EndTime) ||
                        (dto.EndTime > s.StartTime && dto.EndTime <= s.EndTime));

                if (overlapping)
                {
                    return Conflict(new { message = "This sitting overlaps with an existing sitting" });
                }

                var sitting = new Sitting
                {
                    SittingType = dto.SittingType,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    Capacity = dto.Capacity,
                    ClosedForReservations = false
                };

                _context.Sittings.Add(sitting);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetSitting), new { id = sitting.SittingID }, sitting);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating sitting");
                return StatusCode(500, new { message = "Error creating sitting", error = ex.Message });
            }
        }

        // PUT: api/sittings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSitting(int id, SittingUpdateDto dto)
        {
            try
            {
                var sitting = await _context.Sittings.FindAsync(id);
                if (sitting == null)
                {
                    return NotFound(new { message = "Sitting not found" });
                }

                // Only update provided fields
                if (dto.StartTime != default)
                    sitting.StartTime = dto.StartTime;
                if (dto.EndTime != default)
                    sitting.EndTime = dto.EndTime;
                if (!string.IsNullOrEmpty(dto.SittingType))
                    sitting.SittingType = dto.SittingType;
                if (dto.Capacity > 0)
                    sitting.Capacity = dto.Capacity;
                if (dto.ClosedForReservations.HasValue)
                    sitting.ClosedForReservations = dto.ClosedForReservations.Value;

                // Validate that EndTime is after StartTime
                if (sitting.EndTime <= sitting.StartTime)
                {
                    return BadRequest(new { message = "End time must be after start time" });
                }

                await _context.SaveChangesAsync();
                return Ok(new { message = "Sitting updated successfully", sitting });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating sitting");
                return StatusCode(500, new { message = "Error updating sitting", error = ex.Message });
            }
        }

        // DELETE: api/sittings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSitting(int id)
        {
            try
            {
                var sitting = await _context.Sittings
                    .Include(s => s.Reservations)
                    .FirstOrDefaultAsync(s => s.SittingID == id);

                if (sitting == null)
                {
                    return NotFound(new { message = "Sitting not found" });
                }

                // Check if sitting has any reservations
                if (sitting.Reservations.Any())
                {
                    return Conflict(new { message = "Cannot delete sitting with existing reservations" });
                }

                _context.Sittings.Remove(sitting);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Sitting successfully deleted" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting sitting");
                return StatusCode(500, new { message = "Error deleting sitting", error = ex.Message });
            }
        }
    }

    public class SittingCreateDto
    {
        [Required]
        public string SittingType { get; set; } = default!;

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }
    }

    public class SittingUpdateDto
    {
        public string SittingType { get; set; } = default!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        
        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }
        
        public bool? ClosedForReservations { get; set; }
    }
}