namespace SMS.EPA.Criteria.EmployeeEPA
{
    using Fanex.Data.Repository;

    public class UpdateTotalScoreOfProcessCriteria : CriteriaBase
    {
        public int EPAId { get; set; }
        public int ProcessId { get; set; }
        public string AverageScore { get; set; }

        public override bool IsValid() => EPAId > 0 && ProcessId > 0;

        public override string GetSettingKey() => "EpaAuto_UpdateTotalScoreOfProcess";
    }
}