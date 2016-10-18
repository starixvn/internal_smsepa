namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class DeletePeriodCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return PeriodId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaPeriod_Delete";
        }
    }
}