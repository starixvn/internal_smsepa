// ----------------------------------------------------------------------
// <copyright file="EmployeeFlowTargetSettingModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/24/2015">Created.</change>
// </history>

namespace SMS.EPA.Model.CheckFlowAssignment
{
    using System.Collections.Generic;
    using SMS.EPA.Model.FlowManagement;
    using SMS.EPA.Model.TargetEmployeeSetting;

    public class EmployeeFlowTargetSettingModel : TargetEmployeeModel
    {
        public EmployeeFlowTargetSettingModel()
        {
            Evaluators = new List<FlowModel>();
            Reviewers = new List<FlowModel>();
            Managements = new List<FlowModel>();
            HumanResources = new List<FlowModel>();
        }

        public IList<FlowModel> Evaluators { get; set; }

        public IList<FlowModel> Reviewers { get; set; }

        public IList<FlowModel> Managements { get; set; }

        public IList<FlowModel> HumanResources { get; set; }

        public int NumberEvaluators
        {
            get
            {
                return 3;
            }

            set
            {
                NumberEvaluators = value;
            }
        }
    }
}
