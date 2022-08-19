namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Extensions;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.JobLevelStandard;
    using SMS.EPA.Model.JobLevelStandard;
    using SMS.EPA.Model.JobLevelStandard.Period;

    public class JobLevelStandardPeriodController : BaseController
    {
        private readonly IBaseService baseService;

        public JobLevelStandardPeriodController() : this(null)
        {
        }

        public JobLevelStandardPeriodController(IBaseService baseService)
        {
            this.baseService = baseService ?? new BaseService();
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Read)]
        public IList<JobLevelStandardPeriod> GetPeriods(GetJobLevelStandardPeriodCriteria criteria)
        {
            var tuple = baseService.FetchMultiple<JobLevelStandardPeriod, JobLevelStandardPeriodSetting>(criteria);
            IList<JobLevelStandardPeriod> periods = tuple.Item1.ToList();
            foreach(var period in periods)
            {
                period.Settings = tuple.Item2.Where(x => x.PeriodId == period.PeriodId).ToList();
            }

            return periods;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Read)]
        public JobLevelStandardPeriod GetById(int periodId)
        {
            var criteria = new GetJobLevelStandardPeriodByIdCriteria { PeriodId = periodId };
            var tuple = baseService.FetchMultiple<JobLevelStandardPeriod, JobLevelStandardPeriodSetting>(criteria);
            JobLevelStandardPeriod period = tuple.Item1.First();
            period.Settings = tuple.Item2.ToList();

            return period;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Host)]
        public CommandResult Create(CreateJobLevelStandardPeriodCommand command)
        {
            var commandResult = new CommandResult();
            if (command != null)
            {
                command.SettingXml = command.Settings.ToXML().ToString();
                command.IsActive = true;
                command.CreatedBy = SessionService.EmployeeId;
                commandResult = baseService.ExecuteNonQuery(command);
            }

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Host)]
        public CommandResult Edit(EditJobLevelStandardPeriodCommand command)
        {
            var commandResult = new CommandResult();
            if (command != null)
            {
                command.SettingXml = command.Settings.ToXML().ToString();
                command.ModifiedBy = SessionService.EmployeeId;
                commandResult = baseService.ExecuteNonQuery(command);
            }

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Host)]
        public CommandResult EditStatus(EditStatusJobLevelStandardPeriodCommand command)
        {
            var commandResult = new CommandResult();
            if (command != null)
            {
                command.ModifiedBy = SessionService.EmployeeId;
                commandResult = baseService.ExecuteNonQuery(command);
            }

            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Host)]
        public CommandResult Delete(int periodId)
        {
            var criteria = new DeleteJobLevelStandardPeriodCommand
            {
                PeriodId = periodId,
                ModifiedBy = SessionService.EmployeeId
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Read)]
        public IList<PeriodEmployeeSetting> GetEmployeeSettings(GetPeriodEmployeeSettingCriteria criteria)
        {
            var tuple = baseService.FetchMultiple<PeriodEmployeeSetting, PeriodProcessSetting>(criteria);
            IList<PeriodEmployeeSetting> employees = tuple.Item1.ToList();
            foreach(var employee in employees)
            {
                employee.Employee = tuple.Item2.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId && x.ProcessId == JobLevelStandardProcess.Employee);
                employee.Evaluators = tuple.Item2.Where(x => x.EmployeeId == employee.EmployeeId && x.ProcessId == JobLevelStandardProcess.Evaluator).ToList();
                employee.Managements = tuple.Item2.Where(x => x.EmployeeId == employee.EmployeeId && x.ProcessId == JobLevelStandardProcess.Management).ToList();
            }

            return employees;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Host)]
        public CommandResult AddEmployeeSetting(AddPeriodEmployeeSettingCommand command)
        {
            var commandResult = new CommandResult();
            if (command != null)
            {
                command.ModifiedBy = SessionService.EmployeeId;
                commandResult = baseService.ExecuteNonQuery(command);
            }

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Host)]
        public CommandResult EditStatusEmployeeSetting(EditStatusPeriodEmployeeSettingCommand command)
        {
            var commandResult = new CommandResult();
            if (command != null)
            {
                command.ModifiedBy = SessionService.EmployeeId;
                commandResult = baseService.ExecuteNonQuery(command);
            }

            return commandResult;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Read)]
        public IList<JobLevelSettingSummary> GetJobLevelSettings(GetPeriodJobLevelSettingSummaryCriteria criteria)
        {
            return baseService.GetData<JobLevelSettingSummary>(criteria).ToList();
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Read)]
        public IList<JobLevelSetting> GetJobLevelSettingBySectionId(GetPeriodJobLevelSettingBySectionIdCriteria criteria)
        {
            return baseService.GetData<JobLevelSetting>(criteria).ToList();
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Host)]
        public CommandResult EditJobLevelSetting(EditPeriodJobLevelSettingCommand command)
        {
            var commandResult = new CommandResult();
            if (command != null)
            {
                command.SettingXml = command.Settings.ToXML().ToString();
                command.ModifiedBy = SessionService.EmployeeId;
                commandResult = baseService.ExecuteNonQuery(command);
            }

            return commandResult;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Host)]
        public CommandResult CopyDefaultJobLevelSetting(CopyDefaultJobLevelSettingCommand command)
        {
            var commandResult = new CommandResult();
            if (command != null)
            {
                command.ModifiedBy = SessionService.EmployeeId;
                commandResult = baseService.ExecuteNonQuery(command);
            }

            return commandResult;
        }

        public byte GetPermission()
        {
            return SessionService.GetPermission(EPAFunctionKey.JobLevelStandardPeriod);
        }
    }
}