namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetEPAInfoByEmployeeCriteria : CriteriaBase
    {
        public int EPAId { get; set; }

        public int EmployeeId { get; set; }

        public override bool IsValid()
        {
            return EPAId > 0 && EmployeeId > 0;
        }

        public override string GetSettingKey()
        {
            return "EPA_GetInfoByEmployee";
        }
    }
}