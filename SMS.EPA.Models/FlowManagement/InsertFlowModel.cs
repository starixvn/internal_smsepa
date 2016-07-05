namespace SMS.EPA.Model.FlowManagement
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using SMS.EPA.Core.Enumerations;

    public class InsertFlowModel
    {
        private const string ProcessXmlTemplate = "<Process ProcessId='{0}' Duration='{1}' HasSendMail='{2}' RemindDay='{3}'>{4}</Process>";
        private const string EvaluatorXmlTemplate = "<Evaluator EvaluatorId='{0}' Priority='{1}'></Evaluator>";

        public int DepartmentId { get; set; }

        public int FlowTemplateId { get; set; }

        public InsertFlowItemModel Selfs { get; set; }

        public InsertFlowItemModel Evaluators { get; set; }

        public InsertFlowItemModel Reviewers { get; set; }

        public InsertFlowItemModel Managements { get; set; }

        public InsertFlowItemModel HumanResources { get; set; }

        public string FlowDetails
        {
            get
            {
                return ConvertToXml();
            }
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private string ConvertToXml()
        {
            var process = new StringBuilder("<ROOT>");

            if (HumanResources != null)
            {
                var evaluator = new StringBuilder();

                if (HumanResources.Employees.Count > 0)
                {
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

            if (Selfs != null)
            {
                var evaluator = new StringBuilder();
                evaluator.AppendFormat(EvaluatorXmlTemplate, -1, 1);

                process.AppendFormat(
                    ProcessXmlTemplate,
                    (int)ProcessType.Self,
                    Selfs.Duration,
                    Selfs.HasSendMail,
                    Selfs.RemindDay,
                    evaluator);
            }

            if (Evaluators != null)
            {
                var evaluator = new StringBuilder();

                if (Evaluators.Employees.Count > 0)
                {
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

            if (Reviewers != null)
            {
                var evaluator = new StringBuilder();

                if (Reviewers.Employees.Count > 0)
                {
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

            if (Managements != null)
            {
                var evaluator = new StringBuilder();

                if (Managements.Employees.Count > 0)
                {
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

            process.Append("</ROOT>");

            return process.ToString();
        }
    }
}