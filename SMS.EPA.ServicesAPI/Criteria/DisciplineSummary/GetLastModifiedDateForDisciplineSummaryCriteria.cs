namespace SMS.EPA.ServicesAPI.Criteria.DisciplineSummary
{
    using Fanex.Data.Repository;

    public class GetLastModifiedDateForDisciplineSummaryCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Sync_DisciplineSummary_GetLastModifiedDate";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}