namespace SMS.EPA.WindowsService.Criteria
{
    using Fanex.Data.Repository;
    using System;

    public class GetEmployeeCriteria : CriteriaBase
    {
        public DateTime? FromDate { get; set; }

        public override string GetSettingKey()
        {
            return "Employee_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}