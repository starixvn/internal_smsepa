namespace SMS.EPA.Model
{
    using Core.Enumerations;
    using System.Collections.Generic;
    using System;
    using System.Linq;
    using Core.Constants;
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

        public bool IsIncluded { get; set; }

        public TargetTemplate TargetTemplate { get; set; }

        public int NumberEvaluators => SelfEvaluators.Count + Evaluators.Count + Reviewers.Count + Managements.Count;

        public bool IsSemiHasMonthly =>
            Employee != null
            && Employee.HasMonthlyEPA
            && TargetTemplate != null
            && TargetTemplate.TargetTypeId == (int) TargetType.SemiAnnual;

        private string GetEvaluator(IList<PeriodFlowModel> evaluators)
        {
            IEnumerable<string> nickNames = evaluators
                .Where(x => !string.IsNullOrEmpty(x.EvaluatorInfo.NickName))
                .Select(x => x.EvaluatorInfo.NickName);

            string evaluatorNames = string.Join(", ", nickNames);

            return evaluatorNames;
        }

        public object ConvertToExcelRecord()
        {
            var record = new
            {
                Count = 0,
                Department = Employee.Department != null
                    ? Employee.Department.Name
                    : string.Empty,
                Section = Employee.Group != null
                    ? Employee.Group.GroupName
                    : string.Empty,
                Code = Employee.Code,
                FullName = Employee.FullName,
                NickName = Employee.NickName,
                JobTitle = Employee.JobTitle.JobTitleName,
                JoinedDate = Employee.JoinedDate.ToString(Constants.DateFormat),
                Category = Employee.Category,
                Evaluator = GetEvaluator(Evaluators),
                Reviewer = GetEvaluator(Reviewers),
                Management = GetEvaluator(Managements),
                Form = TargetTemplate != null
                    ? TargetTemplate.TargetTypeName
                    : string.Empty
            };

            return record;
        }
    }
}