namespace SMS.EPA.Model
{
    using Core.Constants;
    using Core.Enumerations;
    using Newtonsoft.Json;
    using Report;
    using Resources;
    using System;
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

        public float DisciplineScore { get; set; }

        public EPAEmployeeModel Employee { get; set; }

        public int EPAId { get; set; }

        public int EPAParentId { get; set; }

        public string FinalScore { get; set; }

        [JsonIgnore]
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

                ////// Self
                ////float selfTotalScore;
                ////if (float.TryParse(Process.Self.TotalScore, out selfTotalScore))
                ////{
                ////    finalScore = selfTotalScore + disciplineScore;
                ////}

                // Evaluator
                var processSpecificFinalScore = GetFinalScoreFromProccess(
                    disciplineScore,
                    finalScore,
                    Process.Evaluators,
                    Process.AverageScoreByEvaluators);

                finalScore = SetFinalScore(processSpecificFinalScore, finalScore);

                // Reviewer
                processSpecificFinalScore = GetFinalScoreFromProccess(
                    disciplineScore,
                    finalScore,
                    Process.Reviewers,
                    Process.AverageScoreByReviewers);

                finalScore = SetFinalScore(processSpecificFinalScore, finalScore);

                // Management
                processSpecificFinalScore = GetFinalScoreFromProccess(
                    disciplineScore,
                    finalScore,
                    Process.Managements,
                    Process.AverageScoreByManagements);

                finalScore = SetFinalScore(processSpecificFinalScore, finalScore);

                if (Math.Abs(finalScore + 1) <= Constants.Tolerance)
                {
                    return Lang.NotAvailable;
                }

                return StatusId == (int)EPAStatus.Open ? finalScore.ToString(CultureInfo.InvariantCulture) : FinalScore;
            }
        }

        public string Grade { get; set; }

        public bool HasDisciplineScore { get; set; }

        public bool HasMaxScoreOnKPI { get; set; }

        public bool HasMonthlyEPA { get; set; }

        public bool HasOtherViolation { get; set; }

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

        public bool IsClosed => StatusId == (int)EPAStatus.Closed;

        public bool IsEndProcessBySelf { get; set; }

        public bool IsMonthly => Period.TargetTypeId == (int)TargetType.Monthly;

        public bool IsProbation => Period.TargetTypeId == (int)TargetType.Probation;

        public bool IsPromotion => Period.TargetTypeId == (int)TargetType.Promotion;

        public bool IsSemiAnnual => Period.TargetTypeId == (int)TargetType.SemiAnnual;

        public bool IsSemiHasMonthly => HasMonthlyEPA && Period.TargetTypeId == (int)TargetType.SemiAnnual;

        public IList<EPAKpiGroupModel> KPIGroups { get; set; }

        public float LastDisciplineScore { get; set; }

        public int LastEPAId { get; set; }

        public string LastFinalScore { get; set; }

        public string LastGrade { get; set; }

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

        public IList<EPAProcessModel> LastProcesses { get; set; }

        public float MaxDisciplineScore { get; set; }

        public int MaxFinalScore { get; } = Constants.MaxFinalScore;

        public MonthlyEPAModel MonthlyEPA { get; set; }

        public string OldFinalScore { get; set; }

        public float OtherDisciplines { get; set; }

        public EPAPeriodModel Period { get; set; }

        public EvaluationProcessModel Process { get; set; }

        public RatingTypeModel RatingType { get; set; }

        public EPARecommendationModel Recommendation { get; set; }

        public bool RequiredCommentOnMaxScore { get; set; }

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

        public ScoreRatingTypeModel ScoreRatingType { get; set; }

        public int StatusId { get; set; }

        public float SystemDisciplineScore { get; set; }

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

        private static float GetFinalScoreFromProccess(
            float disciplineScore,
            float finalScore,
            IList<EPAProcessModel> processes,
            float? averageScoreByProcesses)
        {
            float result = finalScore;
            if (processes.Any())
            {
                if (averageScoreByProcesses != null)
                {
                    result = averageScoreByProcesses.Value + disciplineScore;
                }
                else
                {
                    if (processes[0].StatusId == (int)ProcessStatusType.InProgress
                        || processes[0].StatusId == (int)ProcessStatusType.Closed)
                    {
                        result = -1;
                    }
                }
            }

            return result;
        }

        private static float SetFinalScore(float processSpecificFinalScore, float oldFinalScore)
        {
            return Math.Abs(processSpecificFinalScore + 1) > Constants.Tolerance &&
                   Math.Abs(processSpecificFinalScore - oldFinalScore) > Constants.Tolerance
                ? processSpecificFinalScore
                : oldFinalScore;
        }

        private bool IsProcessValid()
        {
            return IsSemiHasMonthly
                   && (Process.CurrentProcess.ProcessId == (int)ProcessType.Self ||
                       Process.CurrentProcess.ProcessId == (int)ProcessType.Evaluator);
        }
    }
}