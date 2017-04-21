namespace SMS.EPA.Criteria
{
    using Core.Constants;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;
    using System.Collections.Generic;
    using System.Text;

    public class SetKPIGroupOfTargetTemplateCriteria : CriteriaBase
    {
        private const string KPIGroupXmlTemplate = "<KPIGroup KPIGroupId='{0}' Weight='{1}' IsSpreadKPI='{2}' DisplayOrder='{3}'></KPIGroup>";

        public int TargetTemplateId { get; set; }

        [SpParam(SqlParamName = "KPIGroups")]
        public string KPIGroupXml => ConvertKPIGroupWeightToXml();

        [SpParam(Ignored = true)]
        public List<KPIGroupModel> KPIGroups { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return TargetTemplateId > 0 && ModifiedBy > 0;
        }

        public override string GetSettingKey()
        {
            return "TargetTemplate_SetKPIGroup";
        }

        private string ConvertKPIGroupWeightToXml()
        {
            string kpiGroupXml = string.Empty;

            if (KPIGroups != null)
            {
                var kpiGroupXmlBuilder = new StringBuilder(Constants.BeginRootTag);

                foreach (KPIGroupModel item in KPIGroups)
                {
                    int spreadKPI = item.IsSpreadKPI ? 1 : 0;
                    kpiGroupXmlBuilder.AppendFormat(KPIGroupXmlTemplate, item.KPIGroupId, item.Weight, spreadKPI, item.DisplayOrder);
                }

                kpiGroupXml = kpiGroupXmlBuilder.Append(Constants.EndRootTag).ToString();
            }

            return kpiGroupXml;
        }
    }
}