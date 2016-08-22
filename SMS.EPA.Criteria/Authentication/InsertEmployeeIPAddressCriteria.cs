namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class InsertEmployeeIPAddressCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public string IPAddress { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaEmployeeIpAddress_Insert";
        }
    }
}