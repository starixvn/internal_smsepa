using System.Collections.Generic;

namespace SMS.EPA.Model
{
    public class EPASummaryRecommendationReportModel
    {
        public PeriodModel Period { get; set; }
        public IEnumerable<EPASummaryRecommendationModel> Recommendations { get; set; }
    }
}
