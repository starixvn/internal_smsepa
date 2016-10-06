namespace SMS.EPA.Model
{
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Services;

    public class EvaluationModel
    {
        public int EPAId { get; set; }

        public int ProcessId { get; set; }

        public int KPIId { get; set; }

        public int EvaluatorId { get; set; }

        public string Code { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string Score { get; set; }

        public string RateEncrypt { get; set; }

        public float Rate { get; set; }

        public string Comment { get; set; }

        public bool IsMaxScore { get; set; }

        public string ScoreToDisplay
        {
            get
            {
                var result = string.Empty;

                if (Score.Length > 0)
                {
                    double score = double.Parse(Score);
                    IUtilityService utilityService = new UtilityService();
                    result = utilityService.SplitNumber(score, Constants.NumberOfDecimalDefault).ToString();
                }

                return result;
            }
        }
    }
}