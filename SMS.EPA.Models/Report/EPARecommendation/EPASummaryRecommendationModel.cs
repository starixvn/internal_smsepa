namespace SMS.EPA.Model
{
    public class EPASummaryRecommendationModel
    {
        public string Code { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string Section { get; set; }

        public string Promotion { get; set; }

        public string AnnualIncrement { get; set; }

        public string JobLevel { get; set; }

        public byte? ProbationStatus { get; set; }
    }
}