// ----------------------------------------------------------------------
// <copyright file="ScoreRatingTypeService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/6/2015">Created.</change>
// </history>

namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.ScoreRatingType;
    using SMS.EPA.Model.ScoreRatingType;
    using SMS.EPA.Services.Interfaces;

    public class ScoreRatingTypeService : BaseService, IScoreRatingTypeService
    {
        public IEnumerable<ScoreRatingTypeModel> GetScoreRatingType(GetScoreRatingTypeCriteria criteria)
        {
            IEnumerable<ScoreRatingTypeModel> scoreRatingTypes = new List<ScoreRatingTypeModel>();

            if (criteria != null && criteria.IsValid())
            {
                scoreRatingTypes = GetData<ScoreRatingTypeModel>(criteria);
            }

            return scoreRatingTypes;
        }
    }
}