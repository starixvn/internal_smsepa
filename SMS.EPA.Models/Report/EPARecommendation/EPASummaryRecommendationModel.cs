namespace SMS.EPA.Model
{
    public class EPASummaryRecommendationModel
    {
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string Section { get; set; }
        public string Team { get; set; }

        public string Promotion { get; set; }
        public string AnnualIncrement { get; set; }
        public string JobLevel { get; set; }
        public byte? ProbationStatus { get; set; }

        public string ModifiedBy { get; set; }
        public string PromotedBy { get; set; }
        public string PayIncreasedBy { get; set; }
        public string JobLevelIncreasedBy { get; set; }
    }
}