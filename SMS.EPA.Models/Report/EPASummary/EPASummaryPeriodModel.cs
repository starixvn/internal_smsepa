// -----------------------------------------------------------------------
// <copyright file="EPASummaryPeriodModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.09.25">Created</change>
// </history>

namespace SMS.EPA.Model.Report.EPASummary
{
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Model.Enitites;

    public class EPASummaryPeriodModel
    {
        public int PeriodId { get; set; }

        public int EmployeeId { get; set; }

        public string FinalScoreEncode { get; set; }

        public string FinalScore { get; set; }

        public bool HasDisciplineScore { get; set; }

        public float DisciplineScore { get; set; }

        public float MaxDisciplineScore { get; set; }

        public string GradeEncode { get; set; }

        public string Grade { get; set; }

        public bool IsEndPeriod { get; set; }

        public StatusModel Status { get; set; }
    }
}