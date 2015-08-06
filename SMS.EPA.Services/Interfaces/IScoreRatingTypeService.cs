// ----------------------------------------------------------------------
// <copyright file="IScoreRatingTypeService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/6/2015">Created.</change>
// </history>

namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Criteria.ScoreRatingType;
    using SMS.EPA.Model.ScoreRatingType;

    public interface IScoreRatingTypeService
    {
        IEnumerable<ScoreRatingTypeModel> GetScoreRatingType(GetScoreRatingTypeCriteria criteria);
    }
}
