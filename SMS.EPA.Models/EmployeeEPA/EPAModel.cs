namespace SMS.EPA.Model
{
    using Core.Constants;
    using Core.Enumerations;
    using Report;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public class EPAModel : IExcelRecord
    {
        public EPAModel()
        {
            KPIGroups = new List<EPAKpiGroupModel>();
            LastProcesses = new List<EPAProcessModel>();
            MonthlyEPA = new MonthlyEPAModel();
            Process = new EvaluationProcessModel();
            Period = new EPAPeriodModel();
            Employee = new EPAEmployeeModel();
            RatingType = new RatingTypeModel();
        }

        public int EPAId { get; set; }

        public int EPAParentId { get; set; }

        public EPAEmployeeModel Employee { get; set; }

        public EPAPeriodModel Period { get; set; }

        public int StatusId { get; set; }

        public ScoreRatingTypeModel ScoreRatingType { get; set; }

        public RatingTypeModel RatingType { get; set; }

        public bool RequiredCommentOnMaxScore { get; set; }

        public string FinalScore { get; set; }

        public string OldFinalScore { get; set; }

        public bool HasMonthlyEPA { get; set; }

        public int MaxFinalScore { get; } = Constants.MaxFinalScore;

        public string Grade { get; set; }

        public bool HasDisciplineScore { get; set; }

        public float DisciplineScore { get; set; }

        public float SystemDisciplineScore { get; set; }

        public float MaxDisciplineScore { get; set; }

        public bool HasMaxScoreOnKPI { get; set; }

        public float OtherDisciplines { get; set; }

        public bool HasOtherViolation { get; set; }

        public EvaluationProcessModel Process { get; set; }

        public IList<EPAKpiGroupModel> KPIGroups { get; set; }

        public MonthlyEPAModel MonthlyEPA { get; set; }

        public EPARecommendationModel Recommendation { get; set; }

        #region Last score

        public int LastEPAId { get; set; }

        public float LastDisciplineScore { get; set; }

        public string LastFinalScore { get; set; }

        public string LastGrade { get; set; }

        public IList<EPAProcessModel> LastProcesses { get; set; }

        public float? LastPerformanceScore
        {
            get
            {
                if (LastProcesses.Count > 0)
                {
                    return EvaluationProcessModel.CalculateAverageScore(LastProcesses);
                }

                return null;
            }
        }

        #endregion Last score

        public int Rowspan
        {
            get
            {
                List<int> numbers = new List<int>
                    {
                        Process.Reviewers.Count,
                        Process.Managements.Count
                    };

                if (!IsSemiHasMonthly)
                {
                    numbers.Add(Process.Evaluators.Count);
                }

                return numbers.Max();
            }
        }

        public bool IsEndProcessBySelf { get; set; }

        public bool IsMonthly => Period.TargetTypeId == (int)TargetType.Monthly;

        public bool IsProbation => Period.TargetTypeId == (int)TargetType.Probation;

        public bool IsPromotion => Period.TargetTypeId == (int)TargetType.Promotion;

        public bool IsSemiAnnual => Period.TargetTypeId == (int)TargetType.SemiAnnual;

        public bool IsSemiHasMonthly => HasMonthlyEPA && Period.TargetTypeId == (int)TargetType.SemiAnnual;

        public bool IsClosed => StatusId == (int)EPAStatus.Closed;

        public bool IsAllowedToEdit
        {
            get
            {
                bool isAllowedToEdit = !(Process.CurrentProcess == null
                                         || Process.CurrentProcess.IsEndProcess
                                         || IsProcessValid()
                                         || StatusId == (int)EPAStatus.Closed);

                return isAllowedToEdit;
            }
        }

        private bool IsProcessValid()
        {
            return IsSemiHasMonthly
                   && (Process.CurrentProcess.ProcessId == (int)ProcessType.Self ||
                       Process.CurrentProcess.ProcessId == (int)ProcessType.Evaluator);
        }

        public string FinalScoreToExport
        {
            get
            {
                // Discipline Score
                float disciplineScore = 0;
                if (HasDisciplineScore)
                {
                    disciplineScore = DisciplineScore;
                }

                float finalScore = -1;

                // Self
                float selfTotalScore;
                if (float.TryParse(Process.Self.TotalScore, out selfTotalScore))
                {
                    finalScore = selfTotalScore + disciplineScore;
                }

                // Evaluator
                if (Process.Evaluators.Any() && Process.AverageScoreByEvaluators != null)
                {
                    finalScore = Process.AverageScoreByEvaluators.Value + disciplineScore;
                }

                // Reviewer
                if (Process.Reviewers.Any() && Process.AverageScoreByReviewers != null)
                {
                    finalScore = Process.AverageScoreByReviewers.Value + disciplineScore;
                }

                // Management
                if (Process.Managements.Any() && Process.AverageScoreByManagements != null)
                {
                    finalScore = Process.AverageScoreByManagements.Value + disciplineScore;
                }

                if (finalScore == -1)
                {
                    return "N/A";
                }

                return StatusId == 0 ? finalScore.ToString(CultureInfo.InvariantCulture) : FinalScore;
            }
        }

        public object ConvertToExcelRecord()
        {
            var record = new
            {
                Count = 0,
                Employee.Code,
                Nick = Employee.NickName,
                Employee.FullName,
                Section = Employee.GroupName,
                JobTitle = Employee.JobTitleName,
                OldFinalScore,
                NewFinalScore = FinalScore
            };

            return record;
        }
    }
}