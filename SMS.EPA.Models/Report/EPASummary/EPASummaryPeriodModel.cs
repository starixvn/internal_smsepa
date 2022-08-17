namespace SMS.EPA.Model
{
    using System.Collections.Generic;
    using Core.Enumerations;
    using Newtonsoft.Json;
    using Report.EPASummary;

    public class EPASummaryPeriodModel
    {
        public int PeriodId { get; set; }

        public int EmployeeId { get; set; }

        public int EPAId { get; set; }

        public byte TargetTypeId { get; set; }

        public bool HasMonthlyEPA { get; set; }

        public string FinalScore { get; set; }

        public bool HasDisciplineScore { get; set; }

        public float DisciplineScore { get; set; }

        public float MaxDisciplineScore { get; set; }

        public string Grade { get; set; }

        public byte StatusId { get; set; }

        public bool IsClosed => StatusId == (byte)EpaStatus.Closed;

        public bool IsSemiHasMonthly => HasMonthlyEPA && TargetTypeId == (byte)TargetType.SemiAnnual;

        [JsonIgnore]
        public IList<EPASummaryDetailScoreModel> DetailScores { get; set; } = new List<EPASummaryDetailScoreModel>();

        [JsonIgnore]
        public MonthlyEPAModel MonthlyEPA { get; set; } = new MonthlyEPAModel();

        [JsonIgnore]
        public ScoreRatingTypeModel ScoreRatingType { get; set; } = new ScoreRatingTypeModel();
    }
}