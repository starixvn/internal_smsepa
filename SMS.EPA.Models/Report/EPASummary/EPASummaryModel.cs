namespace SMS.EPA.Model
{
    using System.Collections.Generic;
    using Core.Constants;
    using Report;

    public class EPASummaryModel : EPAReportConditionalFilename
    {
        public EPASummaryModel()
        {
            PeriodSelecteds = new List<PeriodModel>();
            EPAInfos = new List<EPASummaryEmployeeModel>();
        }

        public IList<PeriodModel> PeriodSelecteds { get; set; }

        public IList<EPASummaryEmployeeModel> EPAInfos { get; set; }

        public int MaxFinalScore => Constants.MaxFinalScore;
    }
}