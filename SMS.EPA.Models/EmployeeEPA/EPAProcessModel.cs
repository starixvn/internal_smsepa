namespace SMS.EPA.Model
{
    using System;

    [Serializable]
    public class EPAProcessModel
    {
        public int EPAId { get; set; }

        public int ProcessId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int StatusEndProcess { get; set; }

        public bool IsEndProcess { get; set; }

        public EPAEmployeeModel EvaluatorInfo { get; set; }

        public string Comment { get; set; }

        public float MaxTotalScore { get; set; }

        public string TotalScore { get; set; }

        public string TotalScoreOriginal
        {
            get
            {
                return TotalScore;
            }
        }

        public string AdjustScore { get; set; }

        public string AdjustScoreOriginal
        {
            get
            {
                return AdjustScore;
            }
        }

        public bool Evaluated { get; set; }

        public DateTime? EvaluateDate { get; set; }

        public float MaxPerformanceScore { get; set; }

        public float MaxSubtractScore { get; set; }

        public float MaxAdditionScore { get; set; }

        public EPAEmployeeModel ModifiedBy { get; set; }
    }
}