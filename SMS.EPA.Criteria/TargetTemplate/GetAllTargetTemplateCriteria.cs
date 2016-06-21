namespace SMS.EPA.Criteria.TargetTemplate
{
    using Fanex.Data.Repository;

    public class GetAllTargetTemplateCriteria : CriteriaBase
    {
        public int? TargetTypeId { get; set; }

        public bool? IsActive { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "TargetTemplate_GetAll";
        }
    }
}