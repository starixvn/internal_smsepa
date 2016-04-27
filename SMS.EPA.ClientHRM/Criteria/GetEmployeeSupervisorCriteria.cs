namespace SMS.EPA.ClientHRM.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class GetEmployeeSupervisorCriteria : CriteriaBase
    {
        public DateTime? FromDate { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeSupervisor_GetForSync";
        }
    }
}
