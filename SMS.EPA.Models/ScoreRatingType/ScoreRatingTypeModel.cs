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

        public float SelfEvaluated { get; set; }

        public float Evaluator { get; set; }

        public float Reviewer { get; set; }

        public float Management { get; set; }

        public float Attendance { get; set; }
    }
}
