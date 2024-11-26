using Microsoft.AspNetCore.Mvc;
using BeanScene.Data;
using Microsoft.EntityFrameworkCore;

namespace BeanScene.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly BeanSceneContext _context;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(BeanSceneContext context, ILogger<DashboardController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("stats")]
        public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
        {
            try
            {
                var today = DateTime.Today;
                var lastMonth = today.AddMonths(-1);

                // Get current period stats
                var currentCustomers = await _context.Guests.CountAsync();
                var currentReservations = await _context.Reservations
                    .Where(r => r.StartTime.Date == today)
                    .CountAsync();

                // Get previous period stats for growth calculation
                var previousCustomers = await _context.Guests
                    .Where(g => g.Reservations.Any(r => r.StartTime < lastMonth))
                    .CountAsync();
                var previousReservations = await _context.Reservations
                    .Where(r => r.StartTime.Date == today.AddDays(-1))
                    .CountAsync();

                // Calculate growth rates
                var customerGrowth = previousCustomers == 0 ? 0 : 
                    ((decimal)(currentCustomers - previousCustomers) / previousCustomers) * 100;
                var reservationGrowth = previousReservations == 0 ? 0 :
                    ((decimal)(currentReservations - previousReservations) / previousReservations) * 100;

                var stats = new DashboardStatsDto
                {
                    TotalCustomers = currentCustomers,
                    TotalReservations = currentReservations,
                    CustomerGrowth = Math.Round(customerGrowth, 1),
                    ReservationGrowth = Math.Round(reservationGrowth, 1),
                    // Orders and Revenue would be added when those features are implemented
                    TotalOrders = 0,
                    Revenue = 0,
                    OrderGrowth = 0
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard statistics");
                return StatusCode(500, new { message = "Error retrieving dashboard statistics" });
            }
        }
    }

    public class DashboardStatsDto
    {
        public int TotalCustomers { get; set; }
        public int TotalReservations { get; set; }
        public int TotalOrders { get; set; }
        public decimal Revenue { get; set; }
        public decimal CustomerGrowth { get; set; }
        public decimal ReservationGrowth { get; set; }
        public decimal OrderGrowth { get; set; }
    }

}