namespace SMS.EPA.Model
{
    using Enitites;
    using System.Collections.Generic;
    using System.Linq;
    using Report;

    public class PersonalFlowModel : IExcelRecord
    {
        public PersonalFlowModel()
        {
            HumanResources = new List<ProcessFlowModel>();
            Evaluators = new List<ProcessFlowModel>();
            Reviewers = new List<ProcessFlowModel>();
            Managements = new List<ProcessFlowModel>();
        }

        public int EmployeeId { get; set; }

        public string Code { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public Group Group { get; set; }

        public IList<ProcessFlowModel> HumanResources { get; set; }

        public IList<ProcessFlowModel> Evaluators { get; set; }

        public IList<ProcessFlowModel> Reviewers { get; set; }

        public IList<ProcessFlowModel> Managements { get; set; }

        private string GetEvaluator(IList<ProcessFlowModel> evaluators)
        {
            IEnumerable<string> nickNames =
                evaluators.Select(x => x.FullName + (!string.IsNullOrEmpty(x.NickName)
                                           ? $" ({x.NickName})"
                                           : string.Empty));
            string evaluatorNames = string.Join(", ", nickNames);

            return evaluatorNames;
        }

        public object ConvertToExcelRecord()
        {
            var record = new
            {
                Count = 0,
                Code = Code,
                FullName = FullName,
                NickName = NickName,
                Section = Group != null ? Group.GroupName : string.Empty,
                HumanResource = GetEvaluator(HumanResources),
                Evaluator = GetEvaluator(Evaluators),
                Reviewer = GetEvaluator(Reviewers),
                Management = GetEvaluator(Managements)
            };

            return record;
        }
    }
}