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
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.TargetTemplate;

    public class TargetEmployeeModel : Employee
    {
        public Group Group { get; set; }

        public TargetTemplateModel TargetTemplate { get; set; }
    }
}
