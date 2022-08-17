namespace SMS.EPA.Model.Report.EPASummary
{
    public class EPASummaryDetailScoreModel
    {
        public int EPAId { get; set; }

        public int PeriodId { get; set; }

        public int EmployeeId { get; set; }

        public int EvaluatorId { get; set; }

        public byte ProcessId { get; set; }

        public byte StatusId { get; set; }

        public string TotalScore { get; set; }

        public string AdjustScore { get; set; }

        public float TotalScoreConverted
        {
            get
            {
                float totalScore = 0;
                if (!string.IsNullOrEmpty(TotalScore))
                {
                    float.TryParse(TotalScore, out totalScore);
                }

                return totalScore;
            }
        }
    }
}