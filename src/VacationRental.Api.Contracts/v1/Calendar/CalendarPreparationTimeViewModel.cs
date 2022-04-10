namespace VacationRental.Api.Contracts.v1.Calendar;

public class CalendarPreparationTimeViewModel
{
    public CalendarPreparationTimeViewModel(int unit)
    {
        Unit = unit;
    }

    public int Unit { get; }
}