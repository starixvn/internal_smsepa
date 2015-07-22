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
    using System;
    using System.Text;
    using SMS.EPA.Core.Enumerations;

    public class InsertFlowModel
    {
        private const string _Process = "<Process ProcessId='{0}' StartDate='{1}' EndDate='{2}' HasSendMail='{3}' IncludeSelfEvaluation='{4}'>{5}</Process>";
        private const string _Evaluator = "<Evaluator EvaluatorId='{0}'></Evaluator>";

        public int GroupId { get; set; }

        public int DepartmentId { get; set; }

        public int FlowType { get; set; }

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

                    if (Selfs.Employees.Count > 0)
                    {
                        foreach (var emp in Selfs.Employees)
                        {
                            evaluator.AppendFormat(_Evaluator, -1);
                        }
                    }

                    process.AppendFormat(_Process, (int)ProcessType.Self, Selfs.StartDate, Selfs.EndDate, Selfs.HasSendMail, Selfs.IncludeSelfEvaluation, evaluator);
                }

                if (Evaluators != null)
                {
                    var evaluator = new StringBuilder();

                    if (Evaluators.Employees.Count > 0)
                    {
                        foreach (var emp in Evaluators.Employees)
                        {
                            evaluator.AppendFormat(_Evaluator, Math.Abs(emp.EmployeeId));
                        }
                    }
                    else
                    {
                        evaluator.AppendFormat(_Evaluator, -1);
                    }

                    process.AppendFormat(_Process, (int)ProcessType.Evaluator, Evaluators.StartDate, Evaluators.EndDate, Evaluators.HasSendMail, Evaluators.IncludeSelfEvaluation, evaluator);
                }

                if (Reviewers != null)
                {
                    var evaluator = new StringBuilder();

                    if (Reviewers.Employees.Count > 0)
                    {
                        foreach (var emp in Reviewers.Employees)
                        {
                            evaluator.AppendFormat(_Evaluator, Math.Abs(emp.EmployeeId));
                        }
                    }
                    else
                    {
                        evaluator.AppendFormat(_Evaluator, -1);
                    }

                    process.AppendFormat(_Process, (int)ProcessType.Reviewer, Reviewers.StartDate, Reviewers.EndDate, Reviewers.HasSendMail, Reviewers.IncludeSelfEvaluation, evaluator);
                }

                if (Managements != null)
                {
                    var evaluator = new StringBuilder();

                    if (Reviewers.Employees.Count > 0)
                    {
                        foreach (var emp in Managements.Employees)
                        {
                            evaluator.AppendFormat(_Evaluator, Math.Abs(emp.EmployeeId));
                        }
                    }
                    else
                    {
                        evaluator.AppendFormat(_Evaluator, -1);
                    }

                    process.AppendFormat(_Process, (int)ProcessType.Management, Managements.StartDate, Managements.EndDate, Managements.HasSendMail, Managements.IncludeSelfEvaluation, evaluator);
                }

                if (HumanResources != null)
                {
                    var evaluator = new StringBuilder();

                    if (HumanResources.Employees.Count > 0)
                    {
                        foreach (var emp in HumanResources.Employees)
                        {
                            evaluator.AppendFormat(_Evaluator, Math.Abs(emp.EmployeeId));
                        }
                    }
                    else
                    {
                        evaluator.AppendFormat(_Evaluator, -1);
                    }

                    process.AppendFormat(_Process, (int)ProcessType.HumanResource, HumanResources.StartDate, HumanResources.EndDate, HumanResources.HasSendMail, HumanResources.IncludeSelfEvaluation, evaluator);
                }

                process.Append("</ROOT>");

                return process.ToString();
            }
        }
    }
}