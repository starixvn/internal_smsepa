// ----------------------------------------------------------------------
// <copyright file="ScoreRatingTypeModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/6/2015">Created.</change>
// </history>

namespace SMS.EPA.Model.ScoreRatingType
{
    public class ScoreRatingTypeModel
    {
        public int ScoreRatingTypeId { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public string Description { get; set; }

        public int SelfEvaluated { get; set; }

        public int Evaluator { get; set; }

        public int Reviewer { get; set; }

        public int Management { get; set; }

        public int Attendance { get; set; }
    }
}
