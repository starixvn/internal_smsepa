namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Model.Enitites;

    public interface IExportEmployeeInfoService
    {
        void Export(IEnumerable<Employee> employees);
    }
}