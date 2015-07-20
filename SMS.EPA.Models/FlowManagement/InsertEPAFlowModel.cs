// -----------------------------------------------------------------------
// <copyright file="InsertEPAFlowModel.cs" company="Nexcel Solutions Viet Nam">
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

    public class InsertEPAFlowModel
    {
        private const string _EPAFlow = "<Process StartDate='{0}' EndDate='{1}' HasSendMail='{2}' ProcessId='{3}' EvaluatorId='{4}'></Process>";

        public int GroupId { get; set; }

        public int DepartmentId { get; set; }

        public int FlowType { get; set; }

        public InsertEPAFlowItemModel Selfs { get; set; }

        public InsertEPAFlowItemModel Evaluators { get; set; }

        public InsertEPAFlowItemModel Reviewers { get; set; }

        public InsertEPAFlowItemModel Managements { get; set; }

        public InsertEPAFlowItemModel HumanResources { get; set; }

        public string FlowDetails
        {
            get
            {
                var value = new StringBuilder("<ROOT>");

                if (Selfs != null)
                {
                    foreach (var self in Selfs.Employees)
                    {
                        value.AppendFormat(_EPAFlow, Selfs.StartDate, Selfs.EndDate, Selfs.HasSendMail, (int)ProcessType.Self, -Math.Abs(self.EmployeeId));
                    }
                }

                if (Evaluators != null)
                {
                    foreach (var evaluator in Evaluators.Employees)
                    {
                        value.AppendFormat(_EPAFlow, Evaluators.StartDate, Evaluators.EndDate, Evaluators.HasSendMail, (int)ProcessType.Evaluator, evaluator.EmployeeId);
                    }
                }

                if (Reviewers != null)
                {
                    foreach (var reviewer in Reviewers.Employees)
                    {
                        value.AppendFormat(_EPAFlow, Reviewers.StartDate, Reviewers.EndDate, Reviewers.HasSendMail, (int)ProcessType.Reviewer, reviewer.EmployeeId);
                    }
                }

                if (Managements != null)
                {
                    foreach (var management in Managements.Employees)
                    {
                        value.AppendFormat(_EPAFlow, Managements.StartDate, Managements.EndDate, Managements.HasSendMail, (int)ProcessType.Management, management.EmployeeId);
                    }
                }

                if (HumanResources != null)
                {
                    foreach (var hr in HumanResources.Employees)
                    {
                        value.AppendFormat(_EPAFlow, HumanResources.StartDate, HumanResources.EndDate, HumanResources.HasSendMail, (int)ProcessType.HumanResource, hr.EmployeeId);
                    }
                }

                value.Append("</ROOT>");

                return value.ToString();
            }
        }
    }
}