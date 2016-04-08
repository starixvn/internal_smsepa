// ----------------------------------------------------------------------
// <copyright file="PeriodEmployeeSettingModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/14/2015">Created.</change>
// </history>

namespace SMS.EPA.Model.Period
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.TargetTemplate;

    [Serializable]
    public class PeriodEmployeeSettingModel
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

        public Employee Employee { get; set; }

        public DepartmentModel Department { get; set; }

        public Group Group { get; set; }

        public JobTitle JobTitle { get; set; }

        public IList<PeriodFlowModel> HumanResources { get; set; }

        public IList<PeriodFlowModel> SelfEvaluators { get; set; }

        public IList<PeriodFlowModel> Evaluators { get; set; }

        public IList<PeriodFlowModel> Reviewers { get; set; }

        public IList<PeriodFlowModel> Managements { get; set; }

        public bool IsIncluded { get; set; }

        public TargetTemplateModel TargetTemplate { get; set; }

        public int NumberEvaluators { get; set; }

        public bool IsSemiHasMonthly
        {
            get
            {
                return Employee.HasMonthlyEPA && TargetTemplate.TargetTypeId == (int)TargetType.SemiAnnual;
            }
        }
    }
}