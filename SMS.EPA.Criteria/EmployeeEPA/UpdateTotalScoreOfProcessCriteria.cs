namespace SMS.EPA.Criteria.EmployeeEPA
{
    using Fanex.Data.Repository;

    public class UpdateTotalScoreOfProcessCriteria : CriteriaBase
    {
        public string DataXml { get; set; }

        public override bool IsValid() => !string.IsNullOrWhiteSpace(DataXml);

        public override string GetSettingKey() => "EpaAuto_UpdateTotalScoreOfProcess";
    }
}