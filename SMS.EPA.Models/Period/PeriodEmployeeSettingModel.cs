namespace SMS.EPA.Model
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Model.Report;

    [Serializable]
    public class PeriodEmployeeSettingModel : IExcelRecord
    {
        public int PeriodId { get; set; }

        public string Name { get; set; }

        public EmployeeViewModel Employee { get; set; }

        public IList<PeriodFlowModel> HumanResources { get; set; } = new List<PeriodFlowModel>();

        public IList<PeriodFlowModel> SelfEvaluators { get; set; } = new List<PeriodFlowModel>();

        public IList<PeriodFlowModel> Evaluators { get; set; } = new List<PeriodFlowModel>();

        public IList<PeriodFlowModel> Reviewers { get; set; } = new List<PeriodFlowModel>();

        public IList<PeriodFlowModel> Managements { get; set; } = new List<PeriodFlowModel>();

        public DateTime? DisciplineStartDate { get; set; }

        public DateTime? DisciplineEndDate { get; set; }

        public bool IsIncluded { get; set; }

        public TargetTemplate TargetTemplate { get; set; }

        public int NumberEvaluators => SelfEvaluators.Count + Evaluators.Count + Reviewers.Count + Managements.Count;

        public bool IsSemiHasMonthly =>
            Employee != null
            && Employee.HasMonthlyEPA
            && TargetTemplate != null
            && TargetTemplate.TargetTypeId == (byte)TargetType.SemiAnnual;

        private static string GetEvaluator(IList<PeriodFlowModel> evaluators)
        {
            var nicknames = evaluators
                .Where(x => !string.IsNullOrEmpty(x.EvaluatorInfo.FullName) || !string.IsNullOrEmpty(x.EvaluatorInfo.NickName))
                .Select(x => string.Format(CultureInfo.InvariantCulture, "- {0} ({1})", x.EvaluatorInfo.FullName, x.EvaluatorInfo.NickName)).ToList();

            return string.Join(Environment.NewLine, nicknames);
        }

        public object ConvertToExcelRecord()
        {
            var group = Employee.Group != null ? Employee.Group.GroupName : string.Empty;
            var form = TargetTemplate != null ? TargetTemplate.TargetTypeName : string.Empty;

            var record = new
            {
                Count = 0,
                Employee.Department,
                Employee.Section,
                Team = group,
                Employee.Code,
                Employee.FullName,
                Employee.NickName,
                JobTitle = Employee.JobTitle.JobTitleName,
                JoinedDate = Employee.JoinedDate.ToString(Constants.DateFormat),
                Employee.Category,
                Evaluator = IsSemiHasMonthly ? string.Empty : GetEvaluator(Evaluators),
                Reviewer = GetEvaluator(Reviewers),
                Management = GetEvaluator(Managements),
                Form = form
            };

            return record;
        }
    }
}