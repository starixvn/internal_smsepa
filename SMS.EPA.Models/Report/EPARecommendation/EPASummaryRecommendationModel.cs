namespace SMS.EPA.Model
{
    using SMS.EPA.Model.Report;

    public class EPASummaryRecommendationModel
    {
        public string Code { get; set; }
        public string NickName { get; set; }
        public string Section { get; set; }
        public string Promotion { get; set; }
        public string AnnualIncrement { get; set; }
        public string JobGrade { get; set; }
        public int? ProbationStatus { get; set; }
    }
}