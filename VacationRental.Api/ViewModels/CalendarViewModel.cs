using System.Collections.Generic;

namespace VacationRental.Api.ViewModels
{
    public class CalendarViewModel
    {
        public CalendarViewModel(int rentalId, List<CalendarDateViewModel> dates)
        {
            RentalId = rentalId;
            Dates = dates;
        }
        
        public int RentalId { get; }
        public List<CalendarDateViewModel> Dates { get; }
    }
}
