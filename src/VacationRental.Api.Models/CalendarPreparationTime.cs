namespace VacationRental.Api.Models;

public class CalendarPreparationTime
{
    public CalendarPreparationTime(int unit)
    {
        Unit = unit;
    }

    public int Unit { get; }
}