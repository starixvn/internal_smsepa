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
            var scoreRatingTypes = GetData<ScoreRatingTypeModel>(criteria);
            return scoreRatingTypes;
        }
    }
}