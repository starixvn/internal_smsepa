namespace SMS.EPA.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class GetDisciplineSummaryForOtherSystemCriteria : CriteriaBase
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TeamIds { get; set; }
        public bool? IsLocal { get; set; }
        public int? EmployeeId { get; set; }
        public short? ReasonId { get; set; }

        public override bool IsValid() => EndDate >= StartDate;

        public override string GetSettingKey() => "DisciplineSummary_GetForOtherSystem";
    }
}