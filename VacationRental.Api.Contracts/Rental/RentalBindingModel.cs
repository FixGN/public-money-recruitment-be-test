using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace VacationRental.Api.Contracts.Rental
{
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class RentalBindingModel
    {
        public RentalBindingModel(int units)
        {
            if (units < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(units), "Units must be greater than 0");
            }

            Units = units;
        }

        /// <summary>Parameterless constructor for System.Text.Json.</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public RentalBindingModel()
        {
        }
        
        [Range(0, int.MaxValue)]
        public int Units { get; set; }
    }
}