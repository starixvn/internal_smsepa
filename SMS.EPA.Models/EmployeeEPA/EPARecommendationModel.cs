namespace SMS.EPA.Model
{
    using SMS.EPA.Core.Enumerations;

    public class EPARecommendationModel
    {
        public int EPAId { get; set; }

        public int? ProbationStatus { get; set; }

        public bool Promoted { get; set; }

        public bool PayIncreased { get; set; }

        public bool JobGradeIncreased { get; set; }

        public JobGradeModel JobGrade { get; set; }

        public bool PassedProbation
        {
            get
            {
                return ProbationStatus.HasValue && ProbationStatus.Value == (int)ProbationStatusType.PassProbation;
            }
        }

        public bool ExtendedProbation
        {
            get
            {
                return ProbationStatus.HasValue && ProbationStatus.Value == (int)ProbationStatusType.ExtendProbation;
            }
        }

        public bool FailedProbation
        {
            get
            {
                return ProbationStatus.HasValue && ProbationStatus.Value == (int)ProbationStatusType.FailProbation;
            }
        }
    }
}