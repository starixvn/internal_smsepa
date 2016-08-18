namespace SMS.EPA.Criteria
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