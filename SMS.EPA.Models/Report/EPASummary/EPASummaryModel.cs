namespace SMS.EPA.Model
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Model.Period;

    public class EPASummaryModel
    {
        public EPASummaryModel()
        {
            PeriodSelecteds = new List<PeriodModel>();
            EPAInfos = new List<EPASummaryEmployeeModel>();
        }

        public IList<PeriodModel> PeriodSelecteds { get; set; }

        public IList<EPASummaryEmployeeModel> EPAInfos { get; set; }

        public int MaxFinalScore
        {
            get
            {
                return Constants.MaxFinalScore;
            }
        }

        public int TotalItem { get; set; }
    }
}