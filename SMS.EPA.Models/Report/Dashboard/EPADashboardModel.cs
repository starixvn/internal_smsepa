// -----------------------------------------------------------------------
// <copyright file="EPADashboardModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.09.04">Created</change>
// </history>

namespace SMS.EPA.Model.Report.Dashboard
{
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.Period;

    public class EPADashboardModel
    {
        public int EPAId { get; set; }

        public StatusModel Status { get; set; }

        public string FinalScoreEncode { get; set; }

        public string FinalScore { get; set; }

        public int MaxFinalScore
        {
            get
            {
                return Constants.MaxFinalScore;
            }
        }

        public bool IsEndProcessBySelf { get; set; }

        public bool HasDisciplineScore { get; set; }

        public float DisciplineScore { get; set; }

        public float MaxDisciplineScore { get; set; }

        public PeriodModel Period { get; set; }
    }
}