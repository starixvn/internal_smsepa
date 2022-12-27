namespace SMS.EPA.Services
{
    using System.Collections.Generic;
    using SMS.EPA.Criteria.JobLevelStandard;
    using SMS.EPA.Model.JobLevelStandard;

    public interface IJobLevelStandardPeriodService
    {
        IList<PeriodEmployeeSetting> GetEmployeeSettings(GetPeriodEmployeeSettingCriteria criteria);
    }
}