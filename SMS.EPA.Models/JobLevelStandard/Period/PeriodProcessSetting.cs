namespace SMS.EPA.Model.JobLevelStandard
{
    using System;
    using SMS.EPA.Core.Enumerations;

    [Serializable]
    public class PeriodProcessSetting
    {
        public JobLevelAssessmentProcess ProcessId { get; set; }
        public int EmployeeId { get; set; }

        public int EvaluatorId { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}