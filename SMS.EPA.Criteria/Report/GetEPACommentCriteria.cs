namespace SMS.EPA.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetEPACommentCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public override string GetSettingKey()
        {
            return "Report_EPAComment";
        }

        public override bool IsValid()
        {
            return PeriodId > 0;
        }
    }
}