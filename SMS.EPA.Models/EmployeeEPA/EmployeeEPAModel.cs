// -----------------------------------------------------------------------
// <copyright file="EmployeeEPAModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.04">Created</change>
// </history>

namespace SMS.EPA.Model.EmployeeEPA
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.TargetType;

    public class EmployeeEPAModel
    {
        public EmployeeEPAModel()
        {
            Evaluators = new List<EmployeeModel>();
        }

        public int EPAId { get; set; }

        public int PeriodId { get; set; }

        public bool IsEndProcess { get; set; }

        public Employee Employee { get; set; }

        public string Score { get; set; }

        public string Comment { get; set; }

        public DepartmentModel Department { get; set; }

        public Group Group { get; set; }

        public TargetTypeModel TargetType { get; set; }

        public StatusModel Status { get; set; }

        public IList<EmployeeModel> Evaluators { get; set; }

        public DateTime? EvaluateDate { get; set; }
    }
}