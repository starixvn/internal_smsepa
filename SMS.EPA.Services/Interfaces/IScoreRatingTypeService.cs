namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria;
    using Model;

    public interface IScoreRatingTypeService
    {
        IEnumerable<ScoreRatingTypeModel> GetScoreRatingType(GetScoreRatingTypeCriteria criteria);
    }
}