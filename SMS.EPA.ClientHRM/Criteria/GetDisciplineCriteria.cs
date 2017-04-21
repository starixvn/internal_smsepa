namespace SMS.EPA.ClientHrm.Criteria
{
    using Fanex.Data.Repository;
    using System;

    public class GetDisciplineCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeDiscipline_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}