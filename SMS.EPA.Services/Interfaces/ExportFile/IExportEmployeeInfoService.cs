namespace SMS.EPA.Services.Interfaces
{
    using SMS.EPA.Criteria;

    public interface IExportEmployeeInfoService
    {
        void Export(GetAllEmployeeCriteria criteria);
    }
}