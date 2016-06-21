namespace SMS.EPA.Criteria.TargetTemplate
{
    using Fanex.Data.Repository;

    public class CreateTargetTemplateCriteria : CriteriaBase
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int? TargetTypeId { get; set; }

        public int ScoreRatingTypeId { get; set; }

        public int RatingTypeId { get; set; }

        public int CreatedBy { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(Name);
        }

        public override string GetSettingKey()
        {
            return "TargetTemplate_Insert";
        }
    }
}