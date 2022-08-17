namespace SMS.EPA.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class CreatePeriodCriteria : CriteriaBase
    {
        public string Name { get; set; }

        public byte TargetTypeId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? AttendanceStartDate { get; set; }

        public DateTime? AttendanceEndDate { get; set; }

        public bool IsActive { get; set; }

        public string Description { get; set; }

        public int CreatedBy { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Name) && TargetTypeId > 0 && CreatedBy > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaPeriod_Insert";
        }
    }
}