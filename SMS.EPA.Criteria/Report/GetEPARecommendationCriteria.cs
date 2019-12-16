namespace SMS.EPA.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetEPARecommendationReportCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public override string GetSettingKey()
        {
            return "Report_EPARecommendation";
        }

        public override bool IsValid()
        {
            return PeriodId > 0;
        }
    }
}