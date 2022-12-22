using SMS.EPA.Core.Enumerations;
using SMS.EPA.Core.Services;
using SMS.EPA.Criteria.JobLevelStandard;
using SMS.EPA.Model.JobLevelStandard;
using SMS.EPA.Services.Interfaces.JobLevelStandard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.EPA.Services.Implement.JobLevelStandard
{
    public class JobLevelStandardPeriodService : IJobLevelStandardPeriodService
    {
        private readonly IBaseService baseService;

        public JobLevelStandardPeriodService() : this(null)
        {
        }

        public JobLevelStandardPeriodService(IBaseService baseService)
        {
            this.baseService = baseService ?? new BaseService();
        }
        public IList<PeriodEmployeeSetting> GetEmployeeSettings(GetPeriodEmployeeSettingCriteria criteria)
        {
            var tuple = baseService.FetchMultiple<PeriodEmployeeSetting, PeriodProcessSetting>(criteria);
            IList<PeriodEmployeeSetting> employees = tuple.Item1.ToList();
            foreach (var employee in employees)
            {
                employee.Employee = tuple.Item2.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId && x.ProcessId == (byte)JobLevelAssessmentProcess.Employee);
                employee.Evaluators = tuple.Item2.Where(x => x.EmployeeId == employee.EmployeeId && x.ProcessId == (byte)JobLevelAssessmentProcess.Evaluator).ToList();
                employee.Managements = tuple.Item2.Where(x => x.EmployeeId == employee.EmployeeId && x.ProcessId == (byte)JobLevelAssessmentProcess.Management).ToList();
            }

            return employees;
        }

        public async Task<IList<PeriodEmployeeSetting>> GetEmployeeSettingsAsync(GetPeriodEmployeeSettingCriteria criteria)
        {
            var tuple = await baseService.FetchMultipleAsync<PeriodEmployeeSetting, PeriodProcessSetting>(criteria);
            IList<PeriodEmployeeSetting> employees = tuple.Item1.ToList();
            foreach (var employee in employees)
            {
                employee.Employee = tuple.Item2.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId && x.ProcessId == (byte)JobLevelAssessmentProcess.Employee);
                employee.Evaluators = tuple.Item2.Where(x => x.EmployeeId == employee.EmployeeId && x.ProcessId == (byte)JobLevelAssessmentProcess.Evaluator).ToList();
                employee.Managements = tuple.Item2.Where(x => x.EmployeeId == employee.EmployeeId && x.ProcessId == (byte)JobLevelAssessmentProcess.Management).ToList();
            }

            return employees;
        }
    }
}
