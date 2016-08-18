namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetKPIOfTargetTemplateCriteria : CriteriaBase
    {
        public int TargetTemplateId { get; set; }

        public override bool IsValid()
        {
            return TargetTemplateId > 0;
        }

        public override string GetSettingKey()
        {
            return "TargetTemplate_GetAllKPIByKPIGroups";
        }
    }
}