namespace SMS.EPA.Model
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Core.Constants;
    using Core.Enumerations;
    using Report;

    [Serializable]
    public class PeriodEmployeeSettingModel : IExcelRecord
    {
        public PeriodEmployeeSettingModel()
        {
            HumanResources = new List<PeriodFlowModel>();
            SelfEvaluators = new List<PeriodFlowModel>();
            Evaluators = new List<PeriodFlowModel>();
            Reviewers = new List<PeriodFlowModel>();
            Managements = new List<PeriodFlowModel>();
        }

        public int PeriodId { get; set; }

        public string Name { get; set; }

        public EmployeeViewModel Employee { get; set; }

        public IList<PeriodFlowModel> HumanResources { get; set; }

        public IList<PeriodFlowModel> SelfEvaluators { get; set; }

        public IList<PeriodFlowModel> Evaluators { get; set; }

        public IList<PeriodFlowModel> Reviewers { get; set; }

        public IList<PeriodFlowModel> Managements { get; set; }

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
            IEnumerable<string> nickNames = evaluators
                .Where(x => !string.IsNullOrEmpty(x.EvaluatorInfo.FullName) || !string.IsNullOrEmpty(x.EvaluatorInfo.NickName))
                .Select(x => string.Format(CultureInfo.InvariantCulture, "{0} ({1})", x.EvaluatorInfo.FullName, x.EvaluatorInfo.NickName));

            string evaluatorNames = string.Join(", ", nickNames);

            return evaluatorNames;
        }

        public object ConvertToExcelRecord()
        {
            string department = Employee.Department != null ? Employee.Department.Name : string.Empty;
            string group = Employee.Group != null ? Employee.Group.GroupName : string.Empty;
            string form = TargetTemplate != null ? TargetTemplate.TargetTypeName : string.Empty;

            var record = new
            {
                Count = 0,
                Department = department,
                Section = group,
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