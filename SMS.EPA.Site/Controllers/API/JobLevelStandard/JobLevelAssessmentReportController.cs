namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.JobLevelStandard;
    using SMS.EPA.Criteria.JobLevelStandard.Report;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.JobLevelStandard;
    using SMS.EPA.Model.JobLevelStandard.Period;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class JobLevelAssessmentReportController : BaseController
    {
        private readonly IBaseService baseService;
        private readonly ICommonService commonService;

        public JobLevelAssessmentReportController() : this(null, null)
        {
        }

        public JobLevelAssessmentReportController(IBaseService baseService, ICommonService commonService)
        {
            this.baseService = baseService ?? new BaseService();
            this.commonService = commonService ?? new CommonService();
        }

        public async Task<IList<JobLevelStandardPeriod>> GetPeriods()
        {
            var criteria = new GetPeriodCriteria();
            var periods = await baseService.GetDataAsync<JobLevelStandardPeriod>(criteria);
            return periods.ToList();
        }

        public IList<MultiSelectItem> GetOrgChart(int? periodId = null)
        {
            var criteria = new Criteria.JobLevelStandard.Report.GetOrgChartByActorIdCriteria
            {
                ActorId = SessionService.EmployeeId,
                PeriodId = periodId
            };
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

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelAssessmentReport, Policy = Policy.Read)]
        public object GetAssessments(GetJobLevelAssessmentReportCriteria criteria)
        {
            var kpicriteria = new GetKPIByPeriodIdCriteria { PeriodId = criteria.PeriodId };
            var kpis = baseService.GetData<JobLevelStandardKPI>(kpicriteria).ToList();

            var settingCriteria = new GetPeriodJobLevelSettingByPeriodIdCriteria { PeriodId = criteria.PeriodId };
            var jobLevelSettings = baseService.GetData<JobLevelSetting>(settingCriteria).ToList();

            criteria.ActorId = SessionService.EmployeeId;
            var tuple = baseService.FetchMultiple<JobLevelAssessment, JobLevelAssessmentKPI>(criteria);
            IList<JobLevelAssessment> assessments = tuple.Item1.ToList();
            foreach (var assessment in assessments)
            {
                assessment.LengthOfService = commonService.CalculateLengthOfService(assessment.JoinedDate, assessment.PeriodStartDate);
                assessment.RequirementMinScore = jobLevelSettings
                    .FirstOrDefault(x => x.SectionId == assessment.SectionId && x.JobLevelId == assessment.CurrentJobLevelId)?.MinWeight;
                assessment.KPIs = tuple.Item2.Where(x => x.EmployeeId == assessment.EmployeeId).ToList();

                if (assessment.TotalWeight.HasValue)
                {
                    assessment.SystemPropose = assessment.TotalWeight.Value - (assessment.RequirementMinScore ?? 0);
                }
            }

            assessments = assessments.OrderBy(x => x.StatusId.HasValue ? 0 : 1).ToList();

            return new { kpis, assessments };
        }
    }
}