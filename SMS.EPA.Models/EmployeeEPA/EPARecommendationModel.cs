namespace SMS.EPA.Model
{
    public class EPARecommendationModel
    {
        public int EPAId { get; set; }

        public int? StatusProbation { get; set; }

        public bool Promoted { get; set; }

        public bool PayIncreased { get; set; }

        public bool JobGradeIncreased { get; set; }

        public JobGradeModel JobGrade { get; set; }
    }
}