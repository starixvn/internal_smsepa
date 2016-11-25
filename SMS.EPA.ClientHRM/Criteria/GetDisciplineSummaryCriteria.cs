namespace SMS.EPA.ClientHrm.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class GetDisciplineSummaryCriteria : CriteriaBase
    {
        public DateTime LastModifiedDate { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeDiscipline_GetSummaryByPeriod";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}