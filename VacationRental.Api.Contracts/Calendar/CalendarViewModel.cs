namespace VacationRental.Api.Contracts.Calendar
{
    public class CalendarViewModel
    {
        public CalendarViewModel(int rentalId, List<CalendarDateViewModel> dates, List<CalendarPreparationTimeViewModel> preparationTimes)
        {
            RentalId = rentalId;
            Dates = dates;
            PreparationTimes = preparationTimes;
        }
        
        public int RentalId { get; }
        public List<CalendarDateViewModel> Dates { get; }
        public List<CalendarPreparationTimeViewModel> PreparationTimes { get; }
    }
}
