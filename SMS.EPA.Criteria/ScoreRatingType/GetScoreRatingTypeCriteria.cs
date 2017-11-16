namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetScoreRatingTypeCriteria : CriteriaBase
    {
        public int? ScoreRatingTypeId { get; set; }

        public bool IsActive { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "EpaScoreRatingType_Get";
    }
}