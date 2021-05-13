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

        public JobLevel JobLevel { get; set; }

        public bool PassedProbation => ProbationStatus.HasValue && ProbationStatus.Value == (int)ProbationStatusType.PassProbation;

        public bool ExtendedProbation => ProbationStatus.HasValue && ProbationStatus.Value == (int)ProbationStatusType.ExtendProbation;

        public bool FailedProbation => ProbationStatus.HasValue && ProbationStatus.Value == (int)ProbationStatusType.FailProbation;
    }
}