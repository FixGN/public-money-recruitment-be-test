namespace VacationRental.Api.Contracts.Common;

public class ErrorViewModel
{
    public ErrorViewModel(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Message cannot be null, empty or whitespace.", nameof(message));
        }
        
        Message = message;
    }

    public string Message { get; }
}