namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetEPASummaryImportCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EPASummaryImport_Get";
        }
    }
}