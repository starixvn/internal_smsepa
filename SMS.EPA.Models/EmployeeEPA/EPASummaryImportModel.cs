// -----------------------------------------------------------------------
// <copyright file="EPASummaryImportModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.10.05">Created</change>
// </history>

namespace SMS.EPA.Model.EmployeeEPA
{
    using SMS.EPA.Core.Services;

    public class EPASummaryImportModel
    {
        public int Id { get; set; }

        public float OriginSelfScore { get; set; }

        public float OriginEvaluatorScore { get; set; }

        public float OriginReviewerScore { get; set; }

        public float OriginFinalScore { get; set; }

        public string OriginFinalGrade { get; set; }

        public string SelfScore
        {
            get
            {
                return EncodeService.Encode(OriginSelfScore.ToString());
            }
        }

        public string EvaluatorScore
        {
            get
            {
                return EncodeService.Encode(OriginEvaluatorScore.ToString());
            }
        }

        public string ReviewerScore
        {
            get
            {
                return EncodeService.Encode(OriginReviewerScore.ToString());
            }
        }

        public string FinalScore
        {
            get
            {
                return EncodeService.Encode(OriginFinalScore.ToString());
            }
        }

        public string FinalGrade
        {
            get
            {
                return EncodeService.Encode(OriginFinalGrade);
            }
        }
    }
}
