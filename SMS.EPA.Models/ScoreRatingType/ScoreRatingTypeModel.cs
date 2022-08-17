namespace SMS.EPA.Model
{
    public class ScoreRatingTypeModel
    {
        public byte ScoreRatingTypeId { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public string Description { get; set; }

        public float SelfEvaluated { get; set; }

        public float Evaluator { get; set; }

        public float Reviewer { get; set; }

        public float Management { get; set; }

        public float Attendance { get; set; }
    }
}