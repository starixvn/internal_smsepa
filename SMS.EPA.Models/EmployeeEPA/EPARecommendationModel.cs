namespace SMS.EPA.Model
{
    using SMS.EPA.Core.Enumerations;

    public class EPARecommendationModel
    {
        public int EPAId { get; set; }

        public int? StatusProbation { get; set; }

        public bool Promoted { get; set; }

        public bool PayIncreased { get; set; }

        public bool JobGradeIncreased { get; set; }

        public JobGradeModel JobGrade { get; set; }

        public bool PassedProbation
        {
            get
            {
                return StatusProbation.HasValue && StatusProbation.Value == (int)StatusProbationTypes.PassProbation;
            }
        }

        public bool ExtendedProbation
        {
            get
            {
                return StatusProbation.HasValue && StatusProbation.Value == (int)StatusProbationTypes.ExtendProbation;
            }
        }

        public bool FailedProbation
        {
            get
            {
                return StatusProbation.HasValue && StatusProbation.Value == (int)StatusProbationTypes.FailProbation;
            }
        }
    }
}