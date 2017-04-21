namespace SMS.EPA.ClientHrm.Criteria
{
    using Fanex.Data.Repository;
    using System;

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