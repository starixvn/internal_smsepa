namespace SMS.EPA.Model.EmployeeEPA
{
    using System;
    using SMS.EPA.Core.Services;

    [Serializable]
    public class EPAProcessModel
    {
        public int EPAId { get; set; }

        public int ProcessId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int StatusEndProcess { get; set; }

        public bool IsEndProcess { get; set; }

        public int EvaluatorId { get; set; }

        public string Code { get; set; }

        public string Avatar
        {
            get
            {
                IUtilityService utilityService = new UtilityService();
                return utilityService.GetAvatar(Code);
            }
        }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public int Priority { get; set; }

        public string Comment { get; set; }

        public float MaxTotalScore { get; set; }

        public string TotalScoreEncode { get; set; }

        public string TotalScore { get; set; }

        public string TotalScoreOriginal
        {
            get
            {
                return TotalScore;
            }
        }

        public string AdjustScoreEncode { get; set; }

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
    }
}