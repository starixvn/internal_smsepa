namespace SMS.EPA.Criteria
{
    using Core.Constants;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;
    using System.Collections.Generic;
    using System.Text;

    public class UpdateTargetTemplateCriteria : CriteriaBase
    {
        private const string KPIXmlTemplate =
            @"<KPI KPIId='{0}' KPIGroupId='{1}' IsSpreadKPI='{2}' Weight='{3}' DisplayOrder='{4}'
                IsRequiredEvaluate='{5}' HasComment='{6}' IsRequiredComment='{7}'></KPI>";

        public int TargetTemplateId { get; set; }

        public string KPIs
        {
            get
            {
                var kpiXml = new StringBuilder(Constants.BeginRootTag);

                if (KPIIds != null)
                {
                    foreach (TargetTemplateKPIModel item in KPIIds)
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

                return kpiXml.Append(Constants.EndRootTag).ToString();
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
    }
}