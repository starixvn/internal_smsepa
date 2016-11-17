namespace SMS.EPA.Services.Interfaces
{
    using SMS.EPA.Criteria;

    public interface IExportEmployeeFlowSettingService
    {
        void Export(GetPeriodEmployeeSettingCriteria criteria);
    }
}