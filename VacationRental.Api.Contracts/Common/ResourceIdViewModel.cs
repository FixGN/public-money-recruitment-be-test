﻿namespace VacationRental.Api.Contracts.Common
{
    public class ResourceIdViewModel
    {
        public ResourceIdViewModel(int id)
        {
            Id = id;
        }
        
        public int Id { get; }
    }
}
