namespace SMS.EPA.Services
{
    using System.Collections.Generic;
    using SMS.EPA.Criteria.JobLevelStandard.Report;
    using SMS.EPA.Model.JobLevelStandard;

    public interface IJoblevelAssessmentReportService
    {
        IList<JobLevelAssessment> GetJoblevelAssessment(GetJobLevelAssessmentReportCriteria criteria);
    }
}