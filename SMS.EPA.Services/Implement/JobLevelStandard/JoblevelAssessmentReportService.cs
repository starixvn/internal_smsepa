namespace SMS.EPA.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.JobLevelStandard;
    using SMS.EPA.Criteria.JobLevelStandard.Report;
    using SMS.EPA.Model.JobLevelStandard;

    public class JoblevelAssessmentReportService : IJoblevelAssessmentReportService
    {
        private readonly IBaseService baseService;
        private readonly ICommonService commonService;

        public JoblevelAssessmentReportService() : this(null, null)
        {
        }

        public JoblevelAssessmentReportService(
            IBaseService baseService,
            ICommonService commonService)
        {
            this.baseService = baseService ?? new BaseService();
            this.commonService = commonService ?? new CommonService();
        }

        public IList<JobLevelAssessment> GetJoblevelAssessment(GetJobLevelAssessmentReportCriteria criteria)
        {
            var settingCriteria = new GetPeriodJobLevelSettingByPeriodIdCriteria { PeriodId = criteria.PeriodId };
            var jobLevelSettings = baseService.GetData<JobLevelSetting>(settingCriteria).ToList();

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

            return assessments;
        }
    }
}