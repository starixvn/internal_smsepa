namespace SMS.EPA.Model.JobLevelStandard
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;
    using SMS.EPA.Core.Enumerations;

    [Serializable]
    public class JobLevelAssessment
    {
        public int PeriodId { get; set; }
        public int? AssessmentId { get; set; }

        public int EmployeeId { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public string Template { get; set; }

        public int SectionId { get; set; }
        public string Section { get; set; }

        public string Team { get; set; }
        public DateTime JoinedDate { get; set; }
        public DateTime PeriodStartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? MilitaryFromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? MilitaryToDate { get; set; }
        public string LengthOfService { get; set; }
        public decimal? TotalWeight { get; set; }
        public decimal? OldTotalWeight { get; set; }
        public int? CurrentJobLevelId { get; set; }
        public string CurrentJobLevel { get; set; }

        public int? NewJobLevelId { get; set; }
        public string NewJobLevel { get; set; }

        public JobLevelAssessmentStatus? StatusId { get; set; }
        public decimal? RequirementMinScore { get; set; }
        public decimal? SystemPropose { get; set; }
        public string Remark { get; set; }
        public IList<PeriodProcessSetting> Evaluators { get; set; } = new List<PeriodProcessSetting>();

        public IList<JobLevelAssessmentKPI> KPIs { get; set; } = new List<JobLevelAssessmentKPI>();

        [JsonIgnore]
        public IList<PeriodProcessSetting> Processes { get; set; } = new List<PeriodProcessSetting>();

        public bool IsHost { get; set; }
        public byte? CurrentProcessId { get; set; }
    }
}