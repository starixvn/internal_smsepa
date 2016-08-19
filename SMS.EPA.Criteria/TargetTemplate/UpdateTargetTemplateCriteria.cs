namespace SMS.EPA.Criteria
{
    using System.Collections.Generic;
    using System.Text;
    using Fanex.Data.Repository;
    using SMS.EPA.Model;

    public class UpdateTargetTemplateCriteria : CriteriaBase
    {
        private const string KPIXmlTemplate =
            @"<KPI KPIId='{0}' KPIGroupId='{1}' IsSpreadKPI='{2}' Weight='{3}' DisplayOrder='{4}'
                IsRequiredEvaluate='{5}' HasComment='{6}' IsRequiredComment='{7}'></KPI>";

        private const string BeginRootTag = "<ROOT>";
        private const string EndRootTag = "</ROOT>";

        public int TargetTemplateId { get; set; }

        public string KPIs
        {
            get
            {
                return ConvertKPIItemToXml(KPIIds);
            }
        }

        public int ModifiedBy { get; set; }

        [SpParam(Ignored = true)]
        public List<TargetTemplateKPIModel> KPIIds { get; set; }

        public override bool IsValid()
        {
            return TargetTemplateId > 0 && ModifiedBy > 0;
        }

        public override string GetSettingKey()
        {
            return "TargetTemplate_Update";
        }

        private string ConvertKPIItemToXml(List<TargetTemplateKPIModel> kpiItems)
        {
            var kpiXml = new StringBuilder(BeginRootTag);

            if (kpiItems != null)
            {
                foreach (TargetTemplateKPIModel item in kpiItems)
                {
                    kpiXml.AppendFormat(
                        KPIXmlTemplate,
                        item.KPIId,
                        item.KPIGroupId,
                        item.IsSpreadKPI,
                        item.Weight,
                        item.DisplayOrder,
                        item.IsRequiredEvaluate,
                        item.HasComment,
                        item.IsRequiredComment);
                }
            }

            return kpiXml.Append(EndRootTag).ToString();
        }
    }
}