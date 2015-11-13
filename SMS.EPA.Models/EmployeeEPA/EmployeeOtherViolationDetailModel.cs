// -----------------------------------------------------------------------
// <copyright file="EmployeeOtherViolationDetailModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.11.13">Created</change>
// </history>

namespace SMS.EPA.Model.EmployeeEPA
{
    using System;

    public class EmployeeOtherViolationDetailModel
    {
        public int EPAId { get; set; }

        public DateTime WorkDate { get; set; }

        public string Name { get; set; }

        public float Score { get; set; }

        public string Comment { get; set; }
    }
}