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
    using SMS.EPA.Core.Services;

    public class EPASummaryPeriodModel
    {
        public int PeriodId { get; set; }

        public int EmployeeId { get; set; }

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

        public int DisciplineScore { get; set; }

        public Status Status { get; set; }
    }
}