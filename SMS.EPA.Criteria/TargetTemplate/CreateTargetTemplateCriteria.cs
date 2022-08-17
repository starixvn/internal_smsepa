namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class CreateTargetTemplateCriteria : CriteriaBase
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public byte TargetTypeId { get; set; }

        public byte ScoreRatingTypeId { get; set; }

        public byte RatingTypeId { get; set; }

        public int CreatedBy { get; set; }

        private bool IsRatingValid()
        {
            return RatingTypeId > 0 && ScoreRatingTypeId > 0;
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(Name)
                   && CreatedBy > 0
                   && TargetTypeId > 0
                   && IsRatingValid();
        }

        public override string GetSettingKey()
        {
            return "TargetTemplate_Insert";
        }
    }
}