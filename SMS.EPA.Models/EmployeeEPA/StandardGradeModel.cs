// -----------------------------------------------------------------------
// <copyright file="StandardGradeModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.10.02">Created</change>
// </history>

namespace SMS.EPA.Model.EmployeeEPA
{
    public class StandardGradeModel
    {
        public int GradeId { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public int LeftScore { get; set; }

        public int RightScore { get; set; }
    }
}