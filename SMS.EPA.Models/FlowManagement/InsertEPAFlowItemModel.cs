// -----------------------------------------------------------------------
// <copyright file="InsertEPAFlowItemModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.20">Created</change>
// </history>

namespace SMS.EPA.Model.FlowManagement
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.Model.Employee;

    public class InsertEPAFlowItemModel
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool HasSendMail { get; set; }

        public bool IncludeSelfEvlauation { get; set; }

        public List<EmployeeModel> Employees { get; set; }
    }
}