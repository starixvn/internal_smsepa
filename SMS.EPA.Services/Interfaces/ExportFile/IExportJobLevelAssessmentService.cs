namespace SMS.EPA.Services.Interfaces.ExportFile
{
    using System.Collections.Generic;
    using System.IO;
    using SMS.EPA.Model.JobLevelStandard;
    public interface IExportJobLevelAssessmentService
    {
        MemoryStream ExportJoblevelAssessmentReport(List<JobLevelAssessmentKPI> kpis, IList<JobLevelAssessment> assessments, string title);
    }
}
