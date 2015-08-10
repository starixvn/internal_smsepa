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
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.TargetTemplate;

    public class EmployeeEPAModel
    {
        public int EmployeeId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string Score { get; set; }

        public string Comment { get; set; }

        public int Permission { get; set; }

        public DepartmentModel Department { get; set; }

        public Group Group { get; set; }

        public TargetTemplateModel Target { get; set; }

        public StatusModel Status { get; set; }

        public EmployeeModel Evaluator { get; set; }

        public DateTime EvaluateDate { get; set; }
    }
}