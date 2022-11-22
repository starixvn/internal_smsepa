namespace SMS.EPA.Model.JobLevelStandard
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.Core.Enumerations;

    [Serializable]
    public class PeriodEmployeeSetting
    {
        public int PeriodId { get; set; }
        public int? AssessmentId { get; set; }
        public int EmployeeId { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string JobLevel { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
        public string Team { get; set; }
        public bool IsExcluded { get; set; }

        public JobLevelAssessmentStatus? StatusId { get; set; }

        public IList<PeriodProcessSetting> Evaluators { get; set; } = new List<PeriodProcessSetting>();
        public PeriodProcessSetting Employee { get; set; }
        public IList<PeriodProcessSetting> Managements { get; set; } = new List<PeriodProcessSetting>();
    }
}