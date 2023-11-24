namespace SMS.EPA.Model.EPAHistory
{
    public class EPAHistoryModel
    {
        public EPAEmployeeInfo Employee { get; set; }

        public EPAPeriodModel Period { get; set; }

        public float FinalScore { get; set; }

        public string EncodeFinalScore { get; set; }

        public string Grade { get; set; }

        public string EncodeGrade { get; set; }

        public bool HasEPAForm { get; set; }

        public int? EPAId { get; set; }

        public bool HasMonthlyEPA { get; set; }
    }
}