﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace VacationRental.Api.Contracts.Rental
{
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class RentalBindingModel
    {
        public RentalBindingModel(int units, int preparationTimeInDays)
        {
            if (units <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(units), "Units must be 0 or greater.");
            }

            if (preparationTimeInDays < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(units), "PreparationTimeInDays must be 0 or greater.");
            }

            Units = units;
            PreparationTimeInDays = preparationTimeInDays;
        }

        /// <summary>Parameterless constructor for System.Text.Json.</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public RentalBindingModel()
        {
        }
        
        [Range(0, int.MaxValue)]
        public int Units { get; set; }
        [Range(0, int.MaxValue)]
        public int PreparationTimeInDays { get; set; }
    }
}