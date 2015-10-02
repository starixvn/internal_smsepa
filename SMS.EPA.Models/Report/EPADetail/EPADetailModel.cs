// -----------------------------------------------------------------------
// <copyright file="EPADetailModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.09.30">Created</change>
// </history>

namespace SMS.EPA.Model.Report.EPADetail
{
    using System.Collections.Generic;
    using SMS.EPA.Model.EmployeeEPA;

    public class EPADetailModel
    {
        public IEnumerable<EPAModel> MemberEPAs { get; set; }

        public IEnumerable<EvaluationPhaseModel> PhaseSelecteds { get; set; }

        public int TotalItem { get; set; }
    }
}