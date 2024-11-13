using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeanScene.Controllers
{
    [Authorize]
    public class ManagerController : Controller
    {
        private readonly ILogger<ManagerController> _logger;

        public ManagerController(ILogger<ManagerController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Policy = "RequireManagerRole")]
        public IActionResult Dashboard()
        {
            _logger.LogInformation("Attempting to access Manager Dashboard");
            _logger.LogInformation($"User Identity: {User.Identity?.Name}");
            _logger.LogInformation($"Is Authenticated: {User.Identity?.IsAuthenticated}");
            _logger.LogInformation($"Claims: {string.Join(", ", User.Claims.Select(c => $"{c.Type}: {c.Value}"))}");

            if (!User.IsInRole("Manager"))
            {
                _logger.LogWarning("Unauthorized access attempt to Manager Dashboard");
                return Forbid();
            }

            return View();
        }
    }

    [Authorize]
    public class StaffController : Controller
    {
        private readonly ILogger<StaffController> _logger;

        public StaffController(ILogger<StaffController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Policy = "RequireStaffRole")]
        public IActionResult Dashboard()
        {
            _logger.LogInformation("Attempting to access Staff Dashboard");
            _logger.LogInformation($"User Identity: {User.Identity?.Name}");
            _logger.LogInformation($"Is Authenticated: {User.Identity?.IsAuthenticated}");
            _logger.LogInformation($"Claims: {string.Join(", ", User.Claims.Select(c => $"{c.Type}: {c.Value}"))}");

            if (!User.IsInRole("Staff") && !User.IsInRole("Manager"))
            {
                _logger.LogWarning("Unauthorized access attempt to Staff Dashboard");
                return Forbid();
            }

            return View();
        }
    }

    [Authorize]
    public class MemberController : Controller
    {
        private readonly ILogger<MemberController> _logger;

        public MemberController(ILogger<MemberController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Policy = "RequireMemberRole")]
        public IActionResult Dashboard()
        {
            _logger.LogInformation("Attempting to access Member Dashboard");
            _logger.LogInformation($"User Identity: {User.Identity?.Name}");
            _logger.LogInformation($"Is Authenticated: {User.Identity?.IsAuthenticated}");
            _logger.LogInformation($"Claims: {string.Join(", ", User.Claims.Select(c => $"{c.Type}: {c.Value}"))}");

            return View();
        }
    }
}