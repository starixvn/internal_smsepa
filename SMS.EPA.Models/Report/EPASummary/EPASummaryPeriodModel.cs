namespace SMS.EPA.Model
{
    public class EPASummaryPeriodModel
    {
        public int PeriodId { get; set; }

        public int EmployeeId { get; set; }

        public string FinalScoreEncrypt { get; set; }

        public string FinalScore { get; set; }

        public bool HasDisciplineScore { get; set; }

        public float DisciplineScore { get; set; }

        public float MaxDisciplineScore { get; set; }

        public string GradeEncrypt { get; set; }

        public string Grade { get; set; }

        public bool IsEndPeriod { get; set; }

        public EPAStatusModel Status { get; set; }
    }
}