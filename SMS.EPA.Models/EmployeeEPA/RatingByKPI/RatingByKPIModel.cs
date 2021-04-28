namespace SMS.EPA.Model
{
    public class RatingByKPIModel
    {
        public int EPAId { get; set; }

        public EPAEmployeeInfo Employee { get; set; }

        public string TotalScore { get; set; }

        public string TotalScoreOriginal => TotalScore;

        public RatingTypeModel RatingType { get; set; }

        public bool RequiredCommentOnMaxScore { get; set; }
        public bool RequiredCommentOnMaxScoreForEvaluator { get; set; }

        public TemporaryKPIGroupModel KPIGroup { get; set; }
    }
}