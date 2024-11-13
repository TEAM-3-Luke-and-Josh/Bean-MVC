using BeanScene.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BeanScene.Services
{
    public interface IReservationService
    {
        SelectList GetAvailableSittings(DateTime? date = null);
        Task<(bool Success, string Message, int ReservationId)> CreateReservationAsync(ReservationViewModel model);
        Task<ReservationViewModel> GetReservationByIdAsync(int id);
        ReservationStepData GetStepData(int currentStep, DateTime? date = null);
    }

    // Add this new class to help manage step data
    public class ReservationStepData
    {
        public int CurrentStep { get; set; }
        public int Progress { get; set; }
        public SelectList AvailableSittings { get; set; }
        public string SelectedSitting { get; set; }
    }
}