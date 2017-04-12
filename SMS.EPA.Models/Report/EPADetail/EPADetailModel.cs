namespace SMS.EPA.Model
{
    using System.Collections.Generic;

    public class EPADetailModel
    {
        public EPADetailModel()
        {
            EpaInfos = new List<EPAModel>();
            PhaseSelecteds = new List<EvaluationPhaseModel>();
        }

        public IEnumerable<EPAModel> EpaInfos { get; set; }

        public IEnumerable<EvaluationPhaseModel> PhaseSelecteds { get; set; }

        public PeriodModel Period { get; set; }

        public string EpaYear => Period?.StartDate?.Year.ToString() ?? string.Empty;

        public string EpaEvaluationPeriod
        {
            get
            {
                if (Period?.StartDate != null)
                {
                    return Period.StartDate.Value.Month <= 6 ? "1H" : "2H";
                }
                return string.Empty;
            }
        }
    }
}