namespace SMS.EPA.Services
{
    using System.Collections.Generic;
    using SMS.EPA.Criteria.JobLevelStandard;
    using SMS.EPA.Criteria.JobLevelStandard.Report;
    using SMS.EPA.Model.JobLevelStandard;

    public interface IJobLevelAssessmentService
    {
        IList<JobLevelSetting> GetJobLevelSettingByPeriod(int periodId);

        IList<JobLevelAssessment> GetJobLevelAssessmentByActor(GetJobLevelAssessmentByActorIdCriteria criteria);

        IList<JobLevelAssessment> GetJobLevelAssessmentReport(GetJobLevelAssessmentReportCriteria criteria);
    }
}