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
    using SMS.EPA.Core.Services;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.Period;

    public class EPADashboardModel
    {
        public int EPAId { get; set; }

        public StatusModel Status { get; set; }

        public string FinalScoreEncode { get; set; }

        public string FinalScore
        {
            get
            {
                return !string.IsNullOrEmpty(FinalScoreEncode) ? EncodeService.Decode(FinalScoreEncode) : string.Empty;
            }

            set
            {
                FinalScoreEncode = EncodeService.Encode(value);
            }
        }

        public bool IsEndProcessBySelf { get; set; }

        public int AttendanceScore
        {
            get
            {
                return 20;
            }
        }

        public PeriodModel Period { get; set; }
    }
}