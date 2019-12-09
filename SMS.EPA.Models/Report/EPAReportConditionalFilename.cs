namespace SMS.EPA.Model.Report
{
    public class EPAReportConditionalFilename
    {
        const byte middleMonth = 6;
        public PeriodModel Period { get; set; }

        public string EpaYear => Period?.StartDate?.Year.ToString() ?? string.Empty;

        public string EpaEvaluationPeriod
        {
            get
            {
                if (Period?.StartDate != null)
                {
                    return Period.StartDate.Value.Month <= middleMonth ? "1H" : "2H";
                }

                return string.Empty;
            }
        }
    }
}
