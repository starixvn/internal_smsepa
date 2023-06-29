namespace SMS.EPA.Site.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using HRM.UserControl.Client.Core.Enumerables;
    using Microsoft.Ajax.Utilities;
    using NPOI.Util;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Extensions;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.JobLevelStandard;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.JobLevelStandard;
    using SMS.EPA.Model.JobLevelStandard.Period;
    using SMS.EPA.Services;

    public class JobLevelAssessmentController : BaseController
    {
        private readonly IBaseService baseService;
        private readonly IJobLevelAssessmentService jobLevelAssessmentService;

        public JobLevelAssessmentController() : this(null, null)
        {
        }

        public JobLevelAssessmentController(
            IBaseService baseService,
            IJobLevelAssessmentService jobLevelAssessmentService)
        {
            this.baseService = baseService ?? new BaseService();
            this.jobLevelAssessmentService = jobLevelAssessmentService ?? new JobLevelAssessmentService();
        }

        public async Task<IList<JobLevelStandardPeriod>> GetPeriods()
        {
            var criteria = new GetPeriodByActorCriteria
            {
                ActorId = CurrentEmployeeId
            };
            var periods = await baseService.GetDataAsync<JobLevelStandardPeriod>(criteria);
            return periods.ToList();
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelAssessment, Policy = Policy.Read)]
        public object GetAssessments(GetJobLevelAssessmentByActorIdCriteria criteria)
        {
            IList<JobLevelSetting> jobLevelSettings = jobLevelAssessmentService.GetJobLevelSettingByPeriod(criteria.PeriodId);

            criteria.ActorId = CurrentEmployeeId;
            IList<JobLevelAssessment> assessments = jobLevelAssessmentService.GetJobLevelAssessmentByActor(criteria);

            IList<JobLevelAssessmentKPI> kpis = assessments
                .SelectMany(ass => ass.KPIs)
                .GroupBy(kpi => new { kpi.KPIName, kpi.KPIId })
                .Select(kpi => kpi.First())
                .OrderBy(kpi => kpi.KPIId)
                .ToList();

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
                    ModifiedBy = CurrentEmployeeId
                };
                commandResult = await baseService.ExecuteNonQueryAsync(command);
                commandResult.ObjectId = command.AssessmentId;
            }

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelAssessment, Policy = Policy.Host)]
        public async Task<CommandResult> EditStatus(EditStatusJobLevelAssessmentCommand command)
        {
            var commandResult = new CommandResult();
            if (command != null)
            {
                command.ModifiedBy = CurrentEmployeeId;
                commandResult = await baseService.ExecuteNonQueryAsync(command);
            }

            return commandResult;
        }

        public IList<MultiSelectItem> GetOrgChart(int? periodId = null)
        {
            var criteria = new GetOrgChartByActorIdCriteria { ActorId = CurrentEmployeeId, PeriodId = periodId };
            var tuple = baseService.FetchMultiple<Department, Section, Team>(criteria);

            var departments = tuple.Item1.ToList();
            var sections = tuple.Item2.ToList();

            var items = departments.Any()
                ? departments.Select(dept => new MultiSelectItem
                {
                    Id = dept.DepartmentId,
                    Text = dept.Name,
                    Items = sections.Where(s => s.DepartmentId.HasValue && s.DepartmentId.Value == dept.DepartmentId).Select(s => new MultiSelectItem
                    {
                        Id = s.SectionId,
                        Text = s.Name,
                        ParentId = dept.DepartmentId,
                        IsLeaf = true
                    }).ToList()
                }).ToList()
                : sections.Select(s => new MultiSelectItem
                {
                    Id = s.SectionId,
                    Text = s.Name,
                    IsLeaf = true
                }).ToList();

            return items;
        }
    }
}
