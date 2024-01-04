namespace SMS.EPA.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.JobLevelStandard;
    using SMS.EPA.Criteria.JobLevelStandard.Report;
    using SMS.EPA.Model.JobLevelStandard;

    public class JobLevelAssessmentService : IJobLevelAssessmentService
    {
        private readonly IBaseService baseService;
        private readonly ICommonService commonService;

        public JobLevelAssessmentService() : this(null, null)
        {
        }

        public JobLevelAssessmentService(IBaseService baseService, ICommonService commonService)
        {
            this.baseService = baseService ?? new BaseService();
            this.commonService = commonService ?? new CommonService();
        }

        public IList<JobLevelSetting> GetJobLevelSettingByPeriod(int periodId)
        {
            var settingCriteria = new GetPeriodJobLevelSettingByPeriodIdCriteria { PeriodId = periodId };
            return baseService.GetData<JobLevelSetting>(settingCriteria).ToList();
        }

        public IList<JobLevelAssessment> GetJobLevelAssessmentByActor(GetJobLevelAssessmentByActorIdCriteria criteria)
        {
            IList<JobLevelSetting> jobLevelSettings = GetJobLevelSettingByPeriod(criteria.PeriodId);

            (var assessments, var processes, var kpis) = baseService.FetchMultiple<JobLevelAssessment, PeriodProcessSetting, JobLevelAssessmentKPI>(criteria);
            foreach (var assessment in assessments)
            {
                assessment.LengthOfService = commonService.CalculateLengthOfService(assessment.JoinedDate, assessment.PeriodStartDate, assessment.MilitaryFromDate, assessment.MilitaryToDate);
                assessment.RequirementMinScore = GetRequirementMinScore(jobLevelSettings, assessment);
                assessment.Processes = processes.Where(x => x.EmployeeId == assessment.EmployeeId).ToList();
                assessment.KPIs = kpis.Where(x => x.EmployeeId == assessment.EmployeeId).ToList();

                SyncWeight(assessment);

                if (assessment.TotalWeight.HasValue)
                {
                    assessment.SystemPropose = assessment.TotalWeight.Value - (assessment.RequirementMinScore ?? 0);
                }

                if (assessment.SystemPropose > 0)
                {
                    GetNewJobLevel(jobLevelSettings, assessment);
                }
            }

            assessments = assessments.OrderBy(x => x.Processes.Any(y => y.EvaluatorId == criteria.ActorId) ? 0 : 1)
                .ThenBy(x => x.StatusId.HasValue ? 0 : 1);

            return assessments.ToList();
        }

        public IList<JobLevelAssessment> GetJobLevelAssessmentReport(GetJobLevelAssessmentReportCriteria criteria)
        {
            IList<JobLevelSetting> jobLevelSettings = GetJobLevelSettingByPeriod(criteria.PeriodId);

            (var assessments, var kpis, var processes) = baseService.FetchMultiple<JobLevelAssessment, JobLevelAssessmentKPI, PeriodProcessSetting>(criteria);
            foreach (var assessment in assessments)
            {
                assessment.Evaluators = processes.Where(x => x.EmployeeId == assessment.EmployeeId).ToList();
                assessment.LengthOfService = commonService.CalculateLengthOfService(assessment.JoinedDate, assessment.PeriodStartDate, assessment.MilitaryFromDate, assessment.MilitaryToDate);
                assessment.RequirementMinScore = jobLevelSettings
                    .FirstOrDefault(x => x.SectionId == assessment.SectionId && x.JobLevelId == assessment.CurrentJobLevelId)?.MinWeight;
                assessment.KPIs = kpis.Where(x => x.EmployeeId == assessment.EmployeeId).ToList();

                SyncWeight(assessment);

                if (assessment.TotalWeight.HasValue)
                {
                    assessment.SystemPropose = assessment.TotalWeight.Value - (assessment.RequirementMinScore ?? 0);
                }

                if (assessment.SystemPropose > 0)
                {
                    GetNewJobLevel(jobLevelSettings, assessment);
                }
            }

            assessments = assessments.OrderBy(x => x.StatusId.HasValue ? 0 : 1);

            return assessments.ToList();
        }

        private static void SyncWeight(JobLevelAssessment assessment)
        {
            if (!assessment.AssessmentId.HasValue && assessment.KPIs.Count > 0)
            {
                var oldTotal = assessment.OldTotalWeight;

                if (oldTotal > 0)
                {
                    assessment.StatusId = JobLevelAssessmentStatus.Pending;
                }

                assessment.TotalWeight = oldTotal;
                foreach (var kpi in assessment.KPIs)
                {
                    kpi.Level = kpi.OldLevel;
                    kpi.Weight = kpi.OldWeight;
                }
            }
        }

        private static decimal? GetRequirementMinScore(IList<JobLevelSetting> jobLevelSettings, JobLevelAssessment assessment)
        {
            return jobLevelSettings.FirstOrDefault(x => x.SectionId == assessment.SectionId && x.JobLevelId == assessment.CurrentJobLevelId)?.MinWeight;
        }

        private static void GetNewJobLevel(IList<JobLevelSetting> jobLevelSettings, JobLevelAssessment assessment)
        {
            int currentJobLevelId = assessment.CurrentJobLevelId ?? 0;
            decimal requirementMinScore = assessment.RequirementMinScore ?? 0;
            IList<JobLevelSetting> settings = jobLevelSettings.Where(x => x.SectionId == assessment.SectionId
                    && x.JobLevelId != currentJobLevelId && x.MinWeight >= requirementMinScore && x.MinWeight <= assessment.TotalWeight)
                .OrderBy(x => x.MinWeight).ToList();
            if (settings.Any() && assessment.RequirementMinScore.HasValue && settings[0].JobLevelId != currentJobLevelId)
            {
                var jobLevel = settings[0];

                assessment.NewJobLevelId = jobLevel.JobLevelId;
                assessment.NewJobLevel = jobLevel.Name;

                if (settings.Count > 1)
                {
                    assessment.NewJobLevel += " > " + settings[settings.Count - 1].Name;
                }
            }
        }
    }
}