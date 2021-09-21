namespace SMS.EPA.Model
{
    using Core.Enumerations;

    public class EPARecommendation
    {
        public int EPAId { get; set; }
        public byte ProcessId { get; set; }

        public short? ProbationStatus { get; set; }

        public bool Promoted { get; set; }
        public bool PayIncreased { get; set; }
        public bool JobLevelIncreased { get; set; }
        public int? JobLevelId { get; set; }
        public string JobLevel { get; set; }

        public string ModifiedBy { get; set; }
        public string PromotedBy { get; set; }
        public string PayIncreasedBy { get; set; }
        public string JobLevelIncreasedBy { get; set; }

        public bool PassedProbation => ProbationStatus.HasValue && ProbationStatus.Value == (byte)ProbationStatusType.PassProbation;
        public bool ExtendedProbation => ProbationStatus.HasValue && ProbationStatus.Value == (byte)ProbationStatusType.ExtendProbation;
        public bool FailedProbation => ProbationStatus.HasValue && ProbationStatus.Value == (byte)ProbationStatusType.FailProbation;
    }
}