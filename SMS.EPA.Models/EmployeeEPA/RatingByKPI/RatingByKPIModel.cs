// -----------------------------------------------------------------------
// <copyright file="RatingByKPIModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.09.09">Created</change>
// </history>

namespace SMS.EPA.Model.EmployeeEPA.RatingByKPI
{
    using SMS.EPA.Model.Enitites;

    public class RatingByKPIModel
    {
        public int EPAId { get; set; }

        public Employee Employee { get; set; }

        public Group Group { get; set; }

        public JobTitle JobTitle { get; set; }

        public string TotalScore { get; set; }

        public TemporaryKPIGroupModel KPIGroup { get; set; }
    }
}