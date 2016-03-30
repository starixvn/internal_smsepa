// ----------------------------------------------------------------------
// <copyright file="CreateTargetTemplateCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/10/2015 11:11:23 AM">Created.</change>
// </history>

namespace SMS.EPA.Criteria.TargetTemplate
{
    using Fanex.Data.Repository;

    public class CreateTargetTemplateCriteria : CriteriaBase
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int? TargetTypeId { get; set; }

        public int ScoreRatingTypeId { get; set; }

        public int RatingTypeId { get; set; }

        public int CreatedBy { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(Name);
        }

        public override string GetSettingKey()
        {
            return "TargetTemplate_Insert";
        }
    }
}