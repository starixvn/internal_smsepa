// ----------------------------------------------------------------------
// <copyright file="TargetEmployeeModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/17/2015">Created.</change>
// </history>

namespace SMS.EPA.Model.TargetEmployeeSetting
{
    using System.Collections.Generic;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.TargetTemplate;

    public class TargetEmployeeModel : Employee
    {
        public TargetEmployeeModel()
        {
            TargetTemplates = new List<TargetTemplateModel>();
        }

        public bool Selected { get; set; }

        public Group Group { get; set; }

        public DepartmentModel Department { get; set; }

        public JobTitle JobTitle { get; set; }

        public TargetTemplateModel RawTargetTemplate { get; set; }

        public IList<TargetTemplateModel> TargetTemplates { get; set; }
    }
}
