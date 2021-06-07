namespace SMS.EPA.WindowsService.Criteria
{
    using Fanex.Data.Repository;
    using System;

    public class GetDisciplineSummaryCriteria : CriteriaBase
    {
        public DateTime LastModifiedDate { get; set; }

        public override string GetSettingKey()
        {
            return "Sync_DisciplineSummary";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}