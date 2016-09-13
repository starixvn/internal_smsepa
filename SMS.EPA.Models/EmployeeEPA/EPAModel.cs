namespace SMS.EPA.Model.EmployeeEPA
{
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Model.ScoreRatingType;

    public class EPAModel
    {
        public EPAModel()
        {
            KPIGroups = new List<EPAKPIGroupModel>();
            LastProcesses = new List<EPAProcessModel>();
            MonthlyEPA = new MonthlyEPAModel();
            Process = new EvaluationProcessModel();
            Period = new EPAPeriodModel();
            Employee = new EPAEmployeeModel();
        }

        public int EPAId { get; set; }

        public int EPAParentId { get; set; }

        public EPAEmployeeModel Employee { get; set; }

        public EPAPeriodModel Period { get; set; }

        public EPAStatusModel Status { get; set; }

        public ScoreRatingTypeModel ScoreRatingType { get; set; }

        public RatingTypeModel RatingType { get; set; }

        public bool RequiredCommentOnMaxScore { get; set; }

        public string FinalScoreEncode { get; set; }

        public string FinalScore { get; set; }

        public bool HasMonthlyEPA { get; set; }

        public int MaxFinalScore
        {
            get
            {
                return Constants.MaxFinalScore;
            }
        }

        public string GradeEncode { get; set; }

        public string Grade { get; set; }

        public bool HasDisciplineScore { get; set; }

        public float DisciplineScore { get; set; }

        public float SystemDisciplineScore { get; set; }

        public float MaxDisciplineScore { get; set; }

        public bool HasMaxScoreOnKPI { get; set; }

        public bool HasOtherViolation { get; set; }

        public EvaluationProcessModel Process { get; set; }

        public IList<EPAKPIGroupModel> KPIGroups { get; set; }

        #region Last score

        public float LastDisciplineScore { get; set; }

        public string LastFinalScoreEncode { get; set; }

        public string LastFinalScore { get; set; }

        public string LastGradeEncode { get; set; }

        public string LastGrade { get; set; }

        public IList<EPAProcessModel> LastProcesses { get; set; }

        public float? LastPerformanceScore
        {
            get
            {
                if (LastProcesses.Count > 0)
                {
                    return Process.CalculateAverageScore(LastProcesses);
                }

                return null;
            }
        }

        #endregion Last score

        public MonthlyEPAModel MonthlyEPA { get; set; }

        public int Rowspan
        {
            get
            {
                List<int> numbers = new List<int>
                    {
                        Process.Evaluators.Count,
                        Process.Reviewers.Count,
                        Process.Managements.Count
                    };

                return numbers.Max();
            }
        }

        public bool IsEndProcessBySelf { get; set; }

        public bool IsMonthly
        {
            get
            {
                return Period != null && Period.TargetTypeId == (int)TargetType.Monthly;
            }
        }

        public bool IsAllowedToEdit
        {
            get
            {
                bool isAllowedToEdit = !(Process.CurrentProcess == null
                    || Process.CurrentProcess.IsEndProcess
                    || (Period.TargetTypeId == (int)TargetType.SemiAnnual
                        && HasMonthlyEPA
                        && (Process.CurrentProcess.ProcessId == (int)ProcessType.Self || Process.CurrentProcess.ProcessId == (int)ProcessType.Evaluator))
                    || Status.StatusId == (int)Core.Enumerations.EPAStatus.Closed
                    || Period.IsEndPeriod);

                return isAllowedToEdit;
            }
        }
    }
}