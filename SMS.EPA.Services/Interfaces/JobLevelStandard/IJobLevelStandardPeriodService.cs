using SMS.EPA.Model.JobLevelStandard;
using System.Collections.Generic;
using System.Threading.Tasks;
using SMS.EPA.Criteria.JobLevelStandard;

namespace SMS.EPA.Services.Interfaces.JobLevelStandard
{
    public interface IJobLevelStandardPeriodService
    {
        Task<IList<PeriodEmployeeSetting>> GetEmployeeSettingsAsync(GetPeriodEmployeeSettingCriteria criteria);
        IList<PeriodEmployeeSetting> GetEmployeeSettings(GetPeriodEmployeeSettingCriteria criteria);
    }
}
