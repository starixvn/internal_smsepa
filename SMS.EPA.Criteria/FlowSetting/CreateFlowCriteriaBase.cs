namespace SMS.EPA.Criteria.FlowSetting
{
    using System.Linq;
    using System.Text;
    using Core.Constants;
    using Core.Enumerations;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;

    public class CreateFlowCriteriaBase : CriteriaBase
    {
        protected const string EvaluatorXmlTemplate = "<Evaluator EvaluatorId='{0}' Priority='{1}'></Evaluator>";
        protected const string ProcessXmlTemplate = "<Process ProcessId='{0}' Duration='{1}' HasSendMail='{2}' RemindDay='{3}'>{4}</Process>";

        [SpParam(Ignored = true)]
        public FlowItemModel Managements { get; set; }

        [SpParam(Ignored = true)]
        public FlowItemModel Reviewers { get; set; }

        [SpParam(Ignored = true)]
        public FlowItemModel Evaluators { get; set; }

        [SpParam(Ignored = true)]
        public FlowItemModel Selfs { get; set; }

        [SpParam(Ignored = true)]
        public FlowItemModel HumanResources { get; set; }

        public int ModifiedBy { get; set; }

        public int FlowTemplateId { get; set; }

        public string FlowDetails
        {
            get
            {
                var process = new StringBuilder(Constants.BeginRootTag);

                AppendHumanResourcesProcess(process);
                AppendSelfProcess(process);
                AppendEvaluatorsProcess(process);
                AppendReviewersProcess(process);

                AppendManagementsProcess(process);

                process.Append(Constants.EndRootTag);

                return process.ToString();
            }
        }

        protected void AppendManagementsProcess(StringBuilder process)
        {
            if (Managements != null && Managements.Employees.Any())
            {
                var evaluator = new StringBuilder();
                for (int i = 0; i < Managements.Employees.Count; i++)
                {
                    evaluator.AppendFormat(EvaluatorXmlTemplate, Managements.Employees[i].EmployeeId, i + 1);
                }

                process.AppendFormat(
                    ProcessXmlTemplate,
                    (int)ProcessType.Management,
                    Managements.Duration,
                    Managements.HasSendMail,
                    Managements.RemindDay,
                    evaluator);
            }
        }

        protected void AppendReviewersProcess(StringBuilder process)
        {
            if (Reviewers != null && Reviewers.Employees.Any())
            {
                var evaluator = new StringBuilder();
                for (int i = 0; i < Reviewers.Employees.Count; i++)
                {
                    evaluator.AppendFormat(EvaluatorXmlTemplate, Reviewers.Employees[i].EmployeeId, i + 1);
                }

                process.AppendFormat(
                    ProcessXmlTemplate,
                    (int)ProcessType.Reviewer,
                    Reviewers.Duration,
                    Reviewers.HasSendMail,
                    Reviewers.RemindDay,
                    evaluator);
            }
        }

        protected void AppendEvaluatorsProcess(StringBuilder process)
        {
            if (Evaluators != null && Evaluators.Employees.Any())
            {
                var evaluator = new StringBuilder();
                for (int i = 0; i < Evaluators.Employees.Count; i++)
                {
                    evaluator.AppendFormat(EvaluatorXmlTemplate, Evaluators.Employees[i].EmployeeId, i + 1);
                }

                process.AppendFormat(
                    ProcessXmlTemplate,
                    (int)ProcessType.Evaluator,
                    Evaluators.Duration,
                    Evaluators.HasSendMail,
                    Evaluators.RemindDay,
                    evaluator);
            }
        }

        protected void AppendSelfProcess(StringBuilder process)
        {
            if (Selfs != null)
            {
                var evaluator = new StringBuilder();
                evaluator.AppendFormat(EvaluatorXmlTemplate, -1, 1);
                process.AppendFormat(ProcessXmlTemplate, (int)ProcessType.Self, Selfs.Duration, Selfs.HasSendMail, Selfs.RemindDay, evaluator);
            }
        }

        protected void AppendHumanResourcesProcess(StringBuilder process)
        {
            if (HumanResources != null && HumanResources.Employees.Any())
            {
                var evaluator = new StringBuilder();
                for (int i = 0; i < HumanResources.Employees.Count; i++)
                {
                    evaluator.AppendFormat(EvaluatorXmlTemplate, HumanResources.Employees[i].EmployeeId, i + 1);
                }

                process.AppendFormat(
                    ProcessXmlTemplate,
                    (int)ProcessType.HumanResource,
                    HumanResources.Duration,
                    HumanResources.HasSendMail,
                    HumanResources.RemindDay,
                    evaluator);
            }
        }

        public override string GetSettingKey()
        {
            throw new System.NotImplementedException();
        }

        public override bool IsValid()
        {
            throw new System.NotImplementedException();
        }
    }
}