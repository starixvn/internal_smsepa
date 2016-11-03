namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
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