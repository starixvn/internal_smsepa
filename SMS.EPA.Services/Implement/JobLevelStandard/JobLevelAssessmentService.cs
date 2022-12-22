namespace SMS.EPA.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
            var today = DateTime.Today;
            IList<JobLevelSetting> jobLevelSettings = GetJobLevelSettingByPeriod(criteria.PeriodId);

            var tuple = baseService.FetchMultiple<JobLevelAssessment, PeriodProcessSetting, JobLevelAssessmentKPI>(criteria);
            IList<JobLevelAssessment> assessments = tuple.Item1.ToList();
            foreach (var assessment in assessments)
            {
                assessment.LengthOfService = commonService.CalculateLengthOfService(assessment.JoinedDate, assessment.PeriodStartDate, assessment.MilitaryFromDate, assessment.MilitaryToDate);
                assessment.RequirementMinScore = GetRequirementMinScore(jobLevelSettings, assessment);
                assessment.Processes = tuple.Item2.Where(x => x.EmployeeId == assessment.EmployeeId).ToList();
                assessment.KPIs = tuple.Item3.Where(x => x.EmployeeId == assessment.EmployeeId).ToList();

                var currentProcess = assessment.Processes
                    .FirstOrDefault(x => x.EvaluatorId == criteria.ActorId && x.FromDate <= today && x.ToDate >= today);
                if (currentProcess != null)
                {
                    assessment.IsHost = true;
                    assessment.CurrentProcessId = currentProcess.ProcessId;
                }

                if (assessment.TotalWeight.HasValue)
                {
                    assessment.SystemPropose = assessment.TotalWeight.Value - (assessment.RequirementMinScore ?? 0);
                }

                GetNewJobLevel(jobLevelSettings, assessment);
            }

            assessments = assessments.OrderBy(x => x.Processes.Any(y => y.EvaluatorId == criteria.ActorId) ? 0 : 1)
                .ThenBy(x => x.StatusId.HasValue ? 0 : 1).ToList();

            return assessments;
        }

        public IList<JobLevelAssessment> GetJobLevelAssessmentReport(GetJobLevelAssessmentReportCriteria criteria)
        {
            IList<JobLevelSetting> jobLevelSettings = GetJobLevelSettingByPeriod(criteria.PeriodId);

            var tuple = baseService.FetchMultiple<JobLevelAssessment, JobLevelAssessmentKPI>(criteria);
            IList<JobLevelAssessment> assessments = tuple.Item1.ToList();
            foreach (var assessment in assessments)
            {
                assessment.LengthOfService = commonService.CalculateLengthOfService(assessment.JoinedDate, assessment.PeriodStartDate, assessment.MilitaryFromDate, assessment.MilitaryToDate);
                assessment.RequirementMinScore = jobLevelSettings
                    .FirstOrDefault(x => x.SectionId == assessment.SectionId && x.JobLevelId == assessment.CurrentJobLevelId)?.MinWeight;
                assessment.KPIs = tuple.Item2.Where(x => x.EmployeeId == assessment.EmployeeId).ToList();

                if (assessment.TotalWeight.HasValue)
                {
                    assessment.SystemPropose = assessment.TotalWeight.Value - (assessment.RequirementMinScore ?? 0);
                }

                GetNewJobLevel(jobLevelSettings, assessment);
            }

            assessments = assessments.OrderBy(x => x.StatusId.HasValue ? 0 : 1).ToList();

            return assessments;
        }

        private static decimal? GetRequirementMinScore(IList<JobLevelSetting> jobLevelSettings, JobLevelAssessment assessment)
        {
            return jobLevelSettings.FirstOrDefault(x => x.SectionId == assessment.SectionId && x.JobLevelId == assessment.CurrentJobLevelId)?.MinWeight;
        }

        private static void GetNewJobLevel(IList<JobLevelSetting> jobLevelSettings, JobLevelAssessment assessment)
        {
            int currentJobLevelId = assessment.CurrentJobLevelId ?? 0;
            if (assessment.SystemPropose > 0)
            {
                decimal requirementMinScore = assessment.RequirementMinScore ?? 0;
                IList<JobLevelSetting> settings = jobLevelSettings.Where(x => x.SectionId == assessment.SectionId
                        && x.JobLevelId != currentJobLevelId && x.MinWeight >= requirementMinScore && x.MinWeight <= assessment.TotalWeight)
                    .OrderBy(x => x.MinWeight).ToList();
                if (settings.Any() && settings.First().JobLevelId != currentJobLevelId)
                {
                    JobLevelSetting jobLevel = settings.First();

                    assessment.NewJobLevelId = jobLevel.JobLevelId;
                    assessment.NewJobLevel = jobLevel.Name;
                }
            }
        }
    }
}