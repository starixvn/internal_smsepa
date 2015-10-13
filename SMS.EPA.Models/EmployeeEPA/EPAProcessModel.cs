// -----------------------------------------------------------------------
// <copyright file="EPAProcessModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.26">Created</change>
// </history>

namespace SMS.EPA.Model.EmployeeEPA
{
    using System;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Model.Enitites;

    public class EPAProcessModel
    {
        public int EPAId { get; set; }

        public int ProcessId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int StatusEndProcess { get; set; }

        public bool IsEndProcess { get; set; }

        public int EvaluatorId { get; set; }

        public string Code { get; set; }

        public string Avatar
        {
            get
            {
                return SMS.EPA.Core.Utilities.Avatar.GetAvatar(Code);
            }
        }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public int Priority { get; set; }

        public string Comment { get; set; }

        public float MaxTotalScore { get; set; }

        public string TotalScoreEncode { get; set; }

        public string LastTotalScoreEncode { get; set; }

        public string LastTotalScore
        {
            get
            {
                return !string.IsNullOrEmpty(LastTotalScoreEncode) ? EncodeService.Decode(LastTotalScoreEncode) : string.Empty;
            }
        }

        public string TotalScore { get; set; }

        public string TotalScoreOriginal
        {
            get
            {
                return TotalScore;
            }
        }

        public string AdjustScoreEncode { get; set; }

        public string AdjustScore { get; set; }

        public string AdjustScoreOriginal
        {
            get
            {
                return AdjustScore;
            }
        }

        public StatusModel Status { get; set; }

        public bool? Evaluated { get; set; }

        public DateTime? EvaluateDate { get; set; }
    }
}