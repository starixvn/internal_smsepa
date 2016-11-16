namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Model;

    public interface IExportEmployeeFlowSettingService
    {
        void Export(IEnumerable<PeriodEmployeeSettingModel> employeeFlowSettings, string periodName);
    }
}