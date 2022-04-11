using VacationRental.Api.Models;

namespace VacationRental.Api.Repositories.Dictionary;

public class DictionaryBookingRepository : IBookingRepository
{
    private readonly IDictionary<int, Booking> _bookingRepository;
    private readonly object _lock = new();
    private readonly IDictionary<int, Rental> _rentalRepository;

    public DictionaryBookingRepository(
        IDictionary<int, Booking> bookingRepository,
        IDictionary<int, Rental> rentalRepository)
    {
        _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
        _rentalRepository = rentalRepository ?? throw new ArgumentNullException(nameof(rentalRepository));
    }

    public Task<Booking?> GetOrDefaultAsync(int id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _bookingRepository.TryGetValue(id, out var booking);

        return Task.FromResult(booking);
    }

    public Task<Booking[]> GetByRentalIdAsync(int rentalId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(_bookingRepository.Values.Where(x => x.RentalId == rentalId).ToArray());
    }

    public Task<Booking[]> GetByRentalIdAndDatePeriodAsync(int rentalId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(_bookingRepository.Values
            .Where(x =>
            {
                var currentEndDate = x.Start.AddDays(x.Nights - 1);
                return x.RentalId == rentalId
                       && x.Start <= endDate
                       && startDate <= currentEndDate;
            })
            .ToArray());
    }

    public Task<Booking> CreateAsync(int rentalId, DateOnly start, int nights, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        lock (_lock)
        {
            // It would be better to do this in a transaction. Due to the fact that code lives there.
            var rental = _rentalRepository[rentalId];
            var bookedUnits = GetByRentalIdAndDatePeriodAsync(
                    rentalId,
                    start.AddDays(-rental.PreparationTimeInDays),
                    start.AddDays(nights + rental.PreparationTimeInDays - 1),
                    cancellationToken).Result
                .Select(x => x.Unit);
            var availableUnit = GetFirstAvailableUnit(bookedUnits, rental.Units);

            var booking = new Booking(_bookingRepository.Count + 1, rentalId, availableUnit, start, nights);
            _bookingRepository.Add(booking.Id, booking);

            return Task.FromResult(booking);
        }
    }
    
    private static int GetFirstAvailableUnit(IEnumerable<int> bookedUnits, int unitsCount) 
        => Enumerable.Range(1, unitsCount).Except(bookedUnits).Min();
}