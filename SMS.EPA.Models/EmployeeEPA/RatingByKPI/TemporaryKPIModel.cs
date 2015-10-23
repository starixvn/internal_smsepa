// -----------------------------------------------------------------------
// <copyright file="TemporaryKPIModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.09.09">Created</change>
// </history>

namespace SMS.EPA.Model.EmployeeEPA.RatingByKPI
{
    public class TemporaryKPIModel
    {
        public int EPAId { get; set; }

        public int KPIId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool HasComment { get; set; }

        public bool IsRequiredComment { get; set; }

        public bool IsRequiredEvaluate { get; set; }

        public double Weight { get; set; }

        public string ScoreEncode { get; set; }

        public string Score { get; set; }

        public bool IsMaxScore { get; set; }

        public string RateEncode { get; set; }

        public int Rate { get; set; }

        public string CommentOriginal
        {
            get
            {
                return Comment;
            }
        }

        public string Comment { get; set; }

        public string AdjustScore { get; set; }

        public int AdjustRate { get; set; }

        public TemporaryKPIGroupModel KPIGroup { get; set; }
    }
}