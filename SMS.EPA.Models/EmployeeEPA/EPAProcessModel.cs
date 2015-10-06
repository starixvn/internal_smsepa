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
        private string _totalScore { get; set; }

        private string _adjusScore { get; set; }

        public int EPAId { get; set; }

        public int ProcessId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int StatusEndProcess { get; set; }

        public bool IsEndProcess { get; set; }

        public int EvaluatorId { get; set; }

        public string Code { get; set; }

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

        public string TotalScore
        {
            get
            {
                return !string.IsNullOrEmpty(TotalScoreEncode) ? EncodeService.Decode(TotalScoreEncode) : string.Empty;
            }

            set
            {
                _totalScore = value;
            }
        }

        public string TotalScoreOriginal
        {
            get
            {
                return TotalScore;
            }
        }

        public string AdjustScoreEncode { get; set; }

        public string AdjustScore
        {
            get
            {
                return !string.IsNullOrEmpty(AdjustScoreEncode) ? EncodeService.Decode(AdjustScoreEncode) : string.Empty;
            }

            set
            {
                _adjusScore = value;
            }
        }

        public StatusModel Status { get; set; }

        public bool? Evaluated { get; set; }

        public DateTime? EvaluateDate { get; set; }

        public string GetTotalScore()
        {
            return _totalScore;
        }

        public string GetAdjustScore()
        {
            return _adjusScore;
        }
    }
}