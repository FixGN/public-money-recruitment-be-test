namespace VacationRental.Api.Contracts.Calendar;

public class CalendarPreparationTimeViewModel
{
    public CalendarPreparationTimeViewModel(int unit)
    {
        Unit = unit;
    }
    public int Unit { get; }
}