using Microsoft.AspNetCore.Mvc;
using BeanScene.Services;
using BeanScene.ViewModels;

namespace BeanScene.Controllers
{
    // Regular MVC Controller for Views
    public class ReservationController : Controller  // Note: singular "Reservation"
    {
        private readonly IReservationService _reservationService;
        private readonly ILogger<ReservationController> _logger;

        public ReservationController(
            IReservationService reservationService,
            ILogger<ReservationController> logger)
        {
            _reservationService = reservationService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new ReservationViewModel
            {
                Date = DateTime.Today,
                NumberOfGuests = 1
            };

            // Get step data (includes ViewBag data)
            var stepData = _reservationService.GetStepData(1);
            
            // Set ViewBag data
            ViewBag.CurrentStep = stepData.CurrentStep;
            ViewBag.Progress = stepData.Progress;
            ViewBag.AvailableSittings = stepData.AvailableSittings;

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Create(ReservationViewModel model, string action)
        {
            int currentStep = int.Parse(Request.Form["CurrentStep"].ToString());
            
            if (action == "previous" && currentStep > 1)
            {
                currentStep--;
            }
            else if (action == "next" && currentStep < 3)
            {
                if (ModelState.IsValid)
                {
                    currentStep++;
                }
            }
            else if (action == "confirm" && currentStep == 3)
            {
                if (ModelState.IsValid)
                {
                    // Process final submission
                    return RedirectToAction("Confirmation", new { id = 1 }); // Replace with actual reservation ID
                }
            }

            var stepData = _reservationService.GetStepData(currentStep, model.Date);
            
            ViewBag.CurrentStep = stepData.CurrentStep;
            ViewBag.Progress = stepData.Progress;
            ViewBag.AvailableSittings = stepData.AvailableSittings;

            return View(model);
        }

        public IActionResult Confirmation(int id)
        {
            // Add confirmation logic
            return View();
        }
    }
}