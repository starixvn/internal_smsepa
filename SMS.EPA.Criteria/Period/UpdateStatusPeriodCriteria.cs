namespace SMS.EPA.Criteria.Period
{
    using Fanex.Data.Repository;

    public class UpdateStatusPeriodCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public bool IsActive { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return PeriodId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaPeriod_UpdateStatus";
        }
    }
}