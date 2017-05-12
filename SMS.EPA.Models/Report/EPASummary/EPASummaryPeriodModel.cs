namespace SMS.EPA.Model
{
    using System.Collections.Generic;
    using Report.EPASummary;

    public class EPASummaryPeriodModel
    {
        public int PeriodId { get; set; }

        public int EmployeeId { get; set; }

        public string FinalScore { get; set; }

        public bool HasDisciplineScore { get; set; }

        public float DisciplineScore { get; set; }

        public float MaxDisciplineScore { get; set; }

        public string Grade { get; set; }

        public int StatusId { get; set; }

        public IList<EPASummaryDetailScoreModel> DetailScores { get; set; } = new List<EPASummaryDetailScoreModel>();
    }
}