namespace SMS.EPA.Model
{
    public class EvaluationModel
    {
        public int EPAId { get; set; }

        public int ProcessId { get; set; }

        public int KPIId { get; set; }

        public int EvaluatorId { get; set; }

        public string Code { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string ScoreEncrypt { get; set; }

        public string Score { get; set; }

        public string RateEncrypt { get; set; }

        public float Rate { get; set; }

        public string Comment { get; set; }

        public bool IsMaxScore { get; set; }
    }
}