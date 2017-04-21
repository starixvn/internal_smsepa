namespace SMS.EPA.Services.Implement
{
    using Core.Services;
    using Criteria;
    using Interfaces;
    using Model;
    using System.Collections.Generic;

    public class ScoreRatingTypeService : BaseService, IScoreRatingTypeService
    {
        public IEnumerable<ScoreRatingTypeModel> GetScoreRatingType(GetScoreRatingTypeCriteria criteria)
        {
            var scoreRatingTypes = GetData<ScoreRatingTypeModel>(criteria);
            return scoreRatingTypes;
        }
    }
}