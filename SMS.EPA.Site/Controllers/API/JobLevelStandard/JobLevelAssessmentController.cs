namespace SMS.EPA.Site.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Extensions;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.JobLevelStandard;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.JobLevelStandard;
    using SMS.EPA.Model.JobLevelStandard.Period;

    public class JobLevelAssessmentController : BaseController
    {
        private readonly IBaseService baseService;

        public JobLevelAssessmentController() : this(null)
        {
        }

        public JobLevelAssessmentController(IBaseService baseService)
        {
            this.baseService = baseService ?? new BaseService();
        }

        public async Task<IList<JobLevelStandardPeriod>> GetPeriods()
        {
            var criteria = new GetPeriodByActorCriteria
            {
                ActorId = SessionService.EmployeeId
            };
            var periods = await baseService.GetDataAsync<JobLevelStandardPeriod>(criteria);
            return periods.ToList();
        }

        [HttpPost]
        public object GetAssessments(GetJobLevelAssessmentByActorIdCriteria criteria)
        {
            var today = DateTime.Today;
            var kpicriteria = new GetKPIByPeriodIdCriteria { PeriodId = criteria.PeriodId };
            var kpis = baseService.GetData<JobLevelStandardKPI>(kpicriteria).ToList();

            var settingCriteria = new GetPeriodJobLevelSettingByPeriodIdCriteria { PeriodId = criteria.PeriodId };
            var jobLevelSettings = baseService.GetData<JobLevelSetting>(settingCriteria).ToList();

            criteria.ActorId = SessionService.EmployeeId;
            var tuple = baseService.FetchMultiple<JobLevelAssessment, PeriodProcessSetting, JobLevelAssessmentKPI>(criteria);
            IList<JobLevelAssessment> assessments = tuple.Item1.ToList();
            foreach(var assessment in assessments)
            {
                assessment.RequirementMinScore = jobLevelSettings
                    .FirstOrDefault(x => x.SectionId == assessment.SectionId && x.JobLevelId == assessment.CurrentJobLevelId)?.MinWeight;
                assessment.Processes = tuple.Item2.Where(x => x.EmployeeId == assessment.EmployeeId).ToList();
                assessment.KPIs = tuple.Item3.Where(x => x.EmployeeId == assessment.EmployeeId).ToList();

                var currentProcess = assessment.Processes
                    .FirstOrDefault(x => x.EvaluatorId == SessionService.EmployeeId && x.FromDate <= today && x.ToDate >= today);
                if(currentProcess != null)
                {
                    assessment.IsHost = true;
                    assessment.CurrentProcessId = currentProcess.ProcessId;
                }

                if (assessment.TotalWeight.HasValue)
                {
                    assessment.SystemPropose = assessment.TotalWeight.Value - (assessment.RequirementMinScore ?? 0);
                }
            }

            return new { kpis, jobLevelSettings, assessments };
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelAssessment, Policy = Policy.Host)]
        public async Task<CommandResult> Edit(JobLevelAssessment assessment)
        {
            var commandResult = new CommandResult();
            if (assessment != null)
            {
                var command = new EditJobLevelAssessmentCommand
                {
                    PeriodId = assessment.PeriodId,
                    EmployeeId = assessment.EmployeeId,
                    TotalWeight = assessment.TotalWeight,
                    CurrentJobLevelId = assessment.CurrentJobLevelId,
                    NewJobLevelId = assessment.NewJobLevelId,
                    StatusId = assessment.StatusId,
                    AssessmentKPIXml = assessment.KPIs.ToXML().ToString(),
                    Remark = assessment.Remark,
                    ModifiedBy = SessionService.EmployeeId
                };
                commandResult = await baseService.ExecuteNonQueryAsync(command);
            }

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Host)]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelAssessment, Policy = Policy.Host)]
        public async Task<CommandResult> EditStatus(EditStatusJobLevelAssessmentCommand command)
        {
            var commandResult = new CommandResult();
            if (command != null)
            {
                command.ModifiedBy = SessionService.EmployeeId;
                commandResult = await baseService.ExecuteNonQueryAsync(command);
            }

            return commandResult;
        }

        public IList<MultiSelectItem> GetOrgChart(int? periodId = null)
        {
            var criteria = new GetOrgChartByActorIdCriteria { ActorId = SessionService.EmployeeId, PeriodId = periodId };
            var tuple = baseService.FetchMultiple<Department, Section, Team>(criteria);

            var departments = tuple.Item1.ToList();
            var sections = tuple.Item2.ToList();
            var teams = tuple.Item3.ToList();

            return GetOrgChart(departments, sections, teams);
        }
    }
}