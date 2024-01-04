namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using HRM.UserControl.Client.Core.Enumerables;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.JobLevelStandard.Report;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.JobLevelStandard;
    using SMS.EPA.Model.JobLevelStandard.Period;
    using SMS.EPA.Services;

    public class JobLevelAssessmentReportController : BaseController
    {
        private readonly IBaseService baseService;
        private readonly IJobLevelAssessmentService jobLevelAssessmentService;

        public JobLevelAssessmentReportController() : this(null, null)
        {
        }

        public JobLevelAssessmentReportController(
            IBaseService baseService,
            IJobLevelAssessmentService jobLevelAssessmentService)
        {
            this.baseService = baseService ?? new BaseService();
            this.jobLevelAssessmentService = jobLevelAssessmentService ?? new JobLevelAssessmentService();
        }

        public async Task<IList<JobLevelStandardPeriod>> GetPeriods()
        {
            var criteria = new GetPeriodCriteria();
            var periods = await baseService.GetDataAsync<JobLevelStandardPeriod>(criteria);
            return periods.ToList();
        }

        public IList<MultiSelectItem> GetOrgChart(int? periodId = null)
        {
            var criteria = new GetOrgChartByActorIdCriteria
            {
                ActorId = CurrentEmployeeId,
                PeriodId = periodId
            };
            (var departments, var sections, _) =  baseService.FetchMultiple<Department, Section, Team>(criteria);

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

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelAssessmentReport, Policy = Policy.Read)]
        public object GetAssessments(GetJobLevelAssessmentReportCriteria criteria)
        {
            criteria.ActorId = CurrentEmployeeId;
            IList<JobLevelAssessment> assessments = jobLevelAssessmentService.GetJobLevelAssessmentReport(criteria);

            IList<JobLevelAssessmentKPI> kpis = assessments
                .SelectMany(ass => ass.KPIs)
                .GroupBy(kpi => new { kpi.KPIName, kpi.KPIId })
                .Select(kpi => kpi.First())
                .OrderBy(kpi => kpi.KPIId)
                .ToList();

            return new { kpis, assessments };
        }
    }
}