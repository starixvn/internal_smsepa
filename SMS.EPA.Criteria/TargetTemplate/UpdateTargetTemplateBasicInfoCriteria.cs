namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class UpdateTargetTemplateBasicInfoCriteria : CriteriaBase
    {
        public int TargetTemplateId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public int ModifiedBy { get; set; }

        public int TargetTypeId { get; set; }

        public int ScoreRatingTypeId { get; set; }

        public int RatingTypeId { get; set; }

        private bool IsRatingValid()
        {
            return RatingTypeId > 0 && ScoreRatingTypeId > 0;
        }

        private bool IsTargetValid()
        {
            return TargetTemplateId > 0 && TargetTypeId > 0;
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(Name)
                   && IsTargetValid()
                   && IsRatingValid()
                   && ModifiedBy > 0;
        }

        public override string GetSettingKey()
        {
            return "TargetTemplate_UpdateBasicInformation";
        }
    }
}