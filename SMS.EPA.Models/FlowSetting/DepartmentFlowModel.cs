namespace SMS.EPA.Model
{
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Model.Report;

    public class DepartmentFlowModel : IExcelRecord
    {
        public int DepartmentId { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }

        public IList<ProcessFlowModel> HumanResources { get; set; } = new List<ProcessFlowModel>();
        public IList<ProcessFlowModel> Evaluators { get; set; } = new List<ProcessFlowModel>();
        public IList<ProcessFlowModel> Reviewers { get; set; } = new List<ProcessFlowModel>();
        public IList<ProcessFlowModel> Managements { get; set; } = new List<ProcessFlowModel>();

        public object ConvertToExcelRecord()
        {
            var record = new
            {
                Count = 0,
                Department,
                Section,
                HumanResource = GetEvaluator(HumanResources),
                Evaluator = GetEvaluator(Evaluators),
                Reviewer = GetEvaluator(Reviewers),
                Management = GetEvaluator(Managements)
            };

            return record;
        }

        private static string GetEvaluator(IList<ProcessFlowModel> evaluators)
        {
            var nicknames = evaluators.Select(x => x.FullName + (!string.IsNullOrEmpty(x.NickName)
                ? $" ({x.NickName})" : string.Empty));

            return string.Join(", ", nicknames);
        }
    }
}