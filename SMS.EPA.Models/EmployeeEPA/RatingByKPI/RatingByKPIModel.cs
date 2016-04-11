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
    public class RatingByKPIModel
    {
        public int EPAId { get; set; }

        public EPAEmployeeModel Employee { get; set; }

        public string TotalScoreEncode { get; set; }

        public string TotalScore { get; set; }

        public string TotalScoreOriginal
        {
            get
            {
                return TotalScore;
            }
        }

        public RatingTypeModel RatingType { get; set; }

        public bool RequiredCommentOnMaxScore { get; set; }

        public TemporaryKPIGroupModel KPIGroup { get; set; }
    }
}