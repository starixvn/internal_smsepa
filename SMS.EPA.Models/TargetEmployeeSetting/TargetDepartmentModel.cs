// ----------------------------------------------------------------------
// <copyright file="TargetDepartmentModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/7/2015">Created.</change>
// </history>

namespace SMS.EPA.Model.TargetEmployeeSetting
{
    using System.Collections.Generic;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.TargetTemplate;

    public class TargetDepartmentModel : DepartmentModel
    {
        public TargetDepartmentModel()
        {
            TargetTemplates = new List<TargetTemplateModel>();
        }

        public TargetTemplateModel RawTargetTemplate { get; set; }

        public IList<TargetTemplateModel> TargetTemplates { get; set; }
    }
}
