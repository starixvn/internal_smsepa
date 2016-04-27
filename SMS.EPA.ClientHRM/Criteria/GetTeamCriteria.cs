namespace SMS.EPA.ClientHRM.Criteria
{
    using Fanex.Data.Repository;
    using System;

    public class GetTeamCriteria : CriteriaBase
    {
        public DateTime? FromDate { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeGroup_Get";
        }
    }
}
