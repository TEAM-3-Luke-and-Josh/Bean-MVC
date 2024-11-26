using Microsoft.AspNetCore.Mvc.Rendering;
using BeanScene.ViewModels;
using BeanScene.Data;

namespace BeanScene.Services
{
    public class ReservationService : IReservationService
    {
        private readonly BeanSceneContext _context;

        public ReservationService(BeanSceneContext context)
        {
            _context = context;
        }

        public ReservationStepData GetStepData(int currentStep, DateTime? date = null)
        {
            return new ReservationStepData
            {
                CurrentStep = currentStep,
                Progress = currentStep * 33, // 33% per step
                AvailableSittings = GetAvailableSittings(date),
                SelectedSitting = default! // This can be populated if needed
            };
        }

        public SelectList GetAvailableSittings(DateTime? date = null)
        {
            date ??= DateTime.Today;
            var startOfDay = date.Value.Date;
            var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

            // First get the data from database
            var sittings = _context.Sittings
                .Where(s => s.StartTime >= startOfDay && 
                            s.StartTime <= endOfDay && 
                            !s.ClosedForReservations)
                .OrderBy(s => s.StartTime)  // Order by start time
                .ToList()  // Execute query here
                .Select(s => new  // Then perform the string formatting
                {
                    Id = s.SittingID,
                    DisplayName = $"{s.SittingType} - {s.StartTime:hh:mm tt} to {s.EndTime:hh:mm tt}"
                })
                .ToList();

            return new SelectList(sittings, "Id", "DisplayName");
        }

        // public async Task<(bool Success, string Message, int ReservationId)> CreateReservationAsync(ReservationViewModel model)
        // {
        //     // Implement reservation creation logic later
        //     // For now, just return success
        //     return (true, "Reservation created successfully", 1);
        // }

        // public async Task<ReservationViewModel> GetReservationByIdAsync(int id)
        // {
        //     // Implement reservation retrieval logic later
        //     // For now, return null
        //     return null;
        // }
    }
}