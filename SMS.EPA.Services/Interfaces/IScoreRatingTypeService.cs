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