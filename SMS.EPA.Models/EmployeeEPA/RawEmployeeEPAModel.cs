﻿// -----------------------------------------------------------------------
// <copyright file="RawEmployeeEPAModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.13">Created</change>
// </history>

namespace SMS.EPA.Model.EmployeeEPA
{
    using SMS.EPA.Model.Employee;

    public class RawEmployeeEPAModel
    {
        public int EPAId { get; set; }

        public EmployeeModel Evaluator { get; set; }
    }
}