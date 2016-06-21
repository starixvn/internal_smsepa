namespace SMS.EPA.Criteria.EPAHistory
{
    using Fanex.Data.Repository;

    public class GetAllPeriodIncludeImportCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaPeriod_GetAllPeriodIncludeImport";
        }
    }
}