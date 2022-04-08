namespace VacationRental.Api.Contracts.v1.Common
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
