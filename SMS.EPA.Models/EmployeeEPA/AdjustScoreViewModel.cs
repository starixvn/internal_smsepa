namespace SMS.EPA.Model
{
    public class AdjustScoreViewModel
    {
        public int EPAId { get; set; }

        public int EmployeeId { get; set; }

        public int ProcessId { get; set; }

        public int EvaluatorId { get; set; }

        public string AdjustScore { get; set; }

        public string TotalScore { get; set; }

        public string Comment { get; set; }
    }
}