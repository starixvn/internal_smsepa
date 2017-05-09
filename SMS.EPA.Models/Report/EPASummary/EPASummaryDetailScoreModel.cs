namespace SMS.EPA.Model.Report.EPASummary
{
    public class EPASummaryDetailScoreModel
    {
        public int EPAId { get; set; }

        public int PeriodId { get; set; }

        public int EmployeeId { get; set; }

        public int ProcessId { get; set; }

        public string TotalScore { get; set; }

        public string AdjustScore { get; set; }
    }
}
