namespace SMS.EPA.Model.EmployeeEPA
{
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Utilities;

    public class EPAStatusModel
    {
        public int StatusId { get; set; }

        public string Name
        {
            get
            {
                return Enumerations.GetDescription((EPAStatus)StatusId);
            }
        }
    }
}