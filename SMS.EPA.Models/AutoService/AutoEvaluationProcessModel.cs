namespace SMS.EPA.Model
{
    using Core.Enumerations;

    public class AutoEvaluationProcessModel
    {
        public int EPAId { get; set; }
        public int PeriodId { get; set; }
        public int TargetTypeId { get; set; }
        public byte ProcessId { get; set; }
        public string TotalScore { get; set; }
        public string AdjustScore { get; set; }
        public bool HasMonthlyEPA { get; set; }

        public bool IsSemiHasMonthly => TargetTypeId == (byte)TargetType.SemiAnnual && HasMonthlyEPA;
    }
}