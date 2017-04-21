namespace SMS.EPA.Criteria
{
    using Core.Constants;
    using Core.Enumerations;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;

    public class CreatePersonalFlowCriteria : CriteriaBase
    {
        private const string ProcessXmlTemplate = "<Process ProcessId='{0}' Duration='{1}' HasSendMail='{2}' RemindDay='{3}'>{4}</Process>";
        private const string EvaluatorXmlTemplate = "<Evaluator EvaluatorId='{0}' Priority='{1}'></Evaluator>";

        public string EmployeeIds { get; set; }

        public int FlowTemplateId { get; set; }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public string FlowDetails
        {
            get
            {
                var process = new StringBuilder(Constants.BeginRootTag);
                int duration;
                int remindDay;
                bool hasSendMail;

                if (HumanResources != null && HumanResources.Employees.Any())
                {
                    var evaluator = new StringBuilder();
                    for (int i = 0; i < HumanResources.Employees.Count; i++)
                    {
                        evaluator.AppendFormat(EvaluatorXmlTemplate, HumanResources.Employees[i].EmployeeId, i + 1);
                    }

                    duration = HumanResources.Duration;
                    hasSendMail = HumanResources.HasSendMail;
                    remindDay = HumanResources.RemindDay;
                    process.AppendFormat(ProcessXmlTemplate, (int)ProcessType.HumanResource, duration, hasSendMail, remindDay, evaluator);
                }

                if (Selfs != null)
                {
                    var evaluator = new StringBuilder();
                    evaluator.AppendFormat(EvaluatorXmlTemplate, -1, 1);
                    process.AppendFormat(ProcessXmlTemplate, (int)ProcessType.Self, Selfs.Duration, Selfs.HasSendMail, Selfs.RemindDay, evaluator);
                }

                if (Evaluators != null && Evaluators.Employees.Any())
                {
                    var evaluator = new StringBuilder();
                    for (int i = 0; i < Evaluators.Employees.Count; i++)
                    {
                        evaluator.AppendFormat(EvaluatorXmlTemplate, Evaluators.Employees[i].EmployeeId, i + 1);
                    }

                    duration = Evaluators.Duration;
                    hasSendMail = Evaluators.HasSendMail;
                    remindDay = Evaluators.RemindDay;
                    process.AppendFormat(ProcessXmlTemplate, (int)ProcessType.Evaluator, duration, hasSendMail, remindDay, evaluator);
                }

                if (Reviewers != null && Reviewers.Employees.Any())
                {
                    var evaluator = new StringBuilder();
                    for (int i = 0; i < Reviewers.Employees.Count; i++)
                    {
                        evaluator.AppendFormat(EvaluatorXmlTemplate, Reviewers.Employees[i].EmployeeId, i + 1);
                    }

                    duration = Reviewers.Duration;
                    hasSendMail = Reviewers.HasSendMail;
                    remindDay = Reviewers.RemindDay;
                    process.AppendFormat(ProcessXmlTemplate, (int)ProcessType.Reviewer, duration, hasSendMail, remindDay, evaluator);
                }

                if (Managements != null && Managements.Employees.Any())
                {
                    var evaluator = new StringBuilder();
                    for (int i = 0; i < Managements.Employees.Count; i++)
                    {
                        evaluator.AppendFormat(EvaluatorXmlTemplate, Managements.Employees[i].EmployeeId, i + 1);
                    }

                    duration = Managements.Duration;
                    hasSendMail = Managements.HasSendMail;
                    remindDay = Managements.RemindDay;
                    process.AppendFormat(ProcessXmlTemplate, (int)ProcessType.Management, duration, hasSendMail, remindDay, evaluator);
                }

                process.Append(Constants.EndRootTag);

                return process.ToString();
            }
        }

        [SpParam(Ignored = true)]
        public FlowItemModel Selfs { get; set; }

        [SpParam(Ignored = true)]
        public FlowItemModel Evaluators { get; set; }

        [SpParam(Ignored = true)]
        public FlowItemModel Reviewers { get; set; }

        [SpParam(Ignored = true)]
        public FlowItemModel Managements { get; set; }

        [SpParam(Ignored = true)]
        public FlowItemModel HumanResources { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(EmployeeIds) && FlowTemplateId > 0 && ModifiedBy > 0;
        }

        public override string GetSettingKey()
        {
            return "SettingEmployeeFlow_Set";
        }
    }
}