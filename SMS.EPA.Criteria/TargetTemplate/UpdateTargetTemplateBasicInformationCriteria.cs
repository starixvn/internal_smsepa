namespace SMS.EPA.Criteria.TargetTemplate
{
    using Fanex.Data.Repository;

    public class UpdateTargetTemplateBasicInformationCriteria : CriteriaBase
    {
        public int TargetTemplateId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public int ModifiedBy { get; set; }

        public int? TargetTypeId { get; set; }

        public int ScoreRatingTypeId { get; set; }

        public int RatingTypeId { get; set; }

        public override bool IsValid()
        {
            return TargetTemplateId > 0;
        }

        public override string GetSettingKey()
        {
            return "TargetTemplate_UpdateBasicInformation";
        }
    }
}