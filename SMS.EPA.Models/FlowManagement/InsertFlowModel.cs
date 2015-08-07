// -----------------------------------------------------------------------
// <copyright file="InsertFlowModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.20">Created</change>
// </history>

namespace SMS.EPA.Model.FlowManagement
{
    using System.Text;
    using SMS.EPA.Core.Enumerations;

    public class InsertFlowModel
    {
        private const string _Process = "<Process ProcessId='{0}' Duration='{1}' HasSendMail='{2}'>{3}</Process>";
        private const string _Evaluator = "<Evaluator EvaluatorId='{0}' Priority='{1}'></Evaluator>";

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
                var process = new StringBuilder("<ROOT>");

                if (Selfs != null)
                {
                    var evaluator = new StringBuilder();
                    evaluator.AppendFormat(_Evaluator, -1, 1);

                    process.AppendFormat(_Process, (int)ProcessType.Self, Selfs.Duration, Selfs.HasSendMail, evaluator);
                }

                if (Evaluators != null)
                {
                    var evaluator = new StringBuilder();

                    if (Evaluators.Employees.Count > 0)
                    {
                        for (int i = 0; i < Evaluators.Employees.Count; i++)
                        {
                            evaluator.AppendFormat(_Evaluator, Evaluators.Employees[i].EmployeeId, i + 1);
                        }
                    }
                    else
                    {
                        evaluator.AppendFormat(_Evaluator, -1, 1);
                    }

                    process.AppendFormat(_Process, (int)ProcessType.Evaluator, Evaluators.Duration, Evaluators.HasSendMail, evaluator);
                }

                if (Reviewers != null)
                {
                    var evaluator = new StringBuilder();

                    if (Reviewers.Employees.Count > 0)
                    {
                        for (int i = 0; i < Reviewers.Employees.Count; i++)
                        {
                            evaluator.AppendFormat(_Evaluator, Reviewers.Employees[i].EmployeeId, i + 1);
                        }
                    }
                    else
                    {
                        evaluator.AppendFormat(_Evaluator, -1, 1);
                    }

                    process.AppendFormat(_Process, (int)ProcessType.Reviewer, Reviewers.Duration, Reviewers.HasSendMail, evaluator);
                }

                if (Managements != null)
                {
                    var evaluator = new StringBuilder();

                    if (Managements.Employees.Count > 0)
                    {
                        for (int i = 0; i < Managements.Employees.Count; i++)
                        {
                            evaluator.AppendFormat(_Evaluator, Managements.Employees[i].EmployeeId, i + 1);
                        }
                    }
                    else
                    {
                        evaluator.AppendFormat(_Evaluator, -1, 1);
                    }

                    process.AppendFormat(_Process, (int)ProcessType.Management, Managements.Duration, Managements.HasSendMail, evaluator);
                }

                if (HumanResources != null)
                {
                    var evaluator = new StringBuilder();

                    if (HumanResources.Employees.Count > 0)
                    {
                        for (int i = 0; i < HumanResources.Employees.Count; i++)
                        {
                            evaluator.AppendFormat(_Evaluator, HumanResources.Employees[i].EmployeeId, i + 1);
                        }
                    }
                    else
                    {
                        evaluator.AppendFormat(_Evaluator, -1, 1);
                    }

                    process.AppendFormat(_Process, (int)ProcessType.HumanResource, HumanResources.Duration, HumanResources.HasSendMail, evaluator);
                }

                process.Append("</ROOT>");

                return process.ToString();
            }
        }
    }
}