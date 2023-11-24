namespace SMS.EPA.Model
{
    using System;

    [Serializable]
    public class EPAProcessModel
    {
        public int EPAId { get; set; }

        public byte ProcessId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public byte StatusId { get; set; }

        public bool IsEndProcess => DateTime.Compare(EndDate.Date, DateTimeOffset.Now.Date) < 0;

        public EPAEmployeeInfo EvaluatorInfo { get; set; } = new EPAEmployeeInfo();

        public string Comment { get; set; }

        public float MaxTotalScore { get; set; }

        public string TotalScore { get; set; }

        public string TotalScoreOriginal => TotalScore;

        public string AdjustScore { get; set; }

        public string AdjustScoreOriginal => AdjustScore;

        public bool Evaluated { get; set; }

        public DateTime? EvaluateDate { get; set; }

        public float MaxPerformanceScore { get; set; }

        public float MaxSubtractScore { get; set; }

        public float MaxAdditionScore { get; set; }

        public EPAEmployeeInfo ModifiedBy { get; set; }
    }
}