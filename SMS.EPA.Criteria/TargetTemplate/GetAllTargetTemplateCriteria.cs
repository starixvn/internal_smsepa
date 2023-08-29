namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetAllTargetTemplateCriteria : CriteriaBase
    {
        public byte? TargetTypeId { get; set; }

        public bool? IsActive { get; set; }

        public string TeamIds { get; set; }

        public byte? ScoreRatingTypeId { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "TargetTemplate_GetAll";
    }
}