namespace SMS.EPA.Model
{
    public class TemporaryKPIModel
    {
        public int EPAId { get; set; }

        public int KPIId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool HasComment { get; set; }

        public bool IsRequiredComment { get; set; }

        public bool IsRequiredEvaluate { get; set; }

        public double Weight { get; set; }

        public bool Evaluated { get; set; }

        public string Score { get; set; }

        public bool IsMaxScore { get; set; }

        public string RateEncrypt { get; set; }

        public float Rate { get; set; }

        public string CommentOriginal => Comment;

        public string Comment { get; set; }

        public string AdjustScore { get; set; }

        public float AdjustRate { get; set; }

        public TemporaryKPIGroupModel KPIGroup { get; set; }
    }
}