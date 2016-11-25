namespace SMS.EPA.ClientHrm.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class GetDepartmentCriteria : CriteriaBase
    {
        public DateTime? FromDate { get; set; }

        public override string GetSettingKey()
        {
            return "Department_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}