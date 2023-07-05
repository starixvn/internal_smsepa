namespace SMS.EPA.Criteria.JobLevelStandard.Template
{
    using System.Collections.Generic;
    using System.Text;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Model.JobLevelStandard.Template;

    public class UpdateJobLevelStandardTemplateKPICriteria : CriteriaBase
    {
        private const string KPIXmlTemplate =
            @"<KPI KPIId='{0}' DisplayOrder='{1}' Weight='{2}'></KPI>";

        public int TemplateId { get; set; }

        public string KPIs
        {
            get
            {
                var kpiXml = new StringBuilder(Constants.BeginRootTag);

                if (KPIIds != null)
                {
                    foreach (JobLevelStandardTemplateKPI item in KPIIds)
                    {
                        kpiXml.AppendFormat(
                            KPIXmlTemplate,
                            item.KPIId,
                            item.DisplayOrder,
                            item.Weight);
                    }
                }

                return kpiXml.Append(Constants.EndRootTag).ToString();
            }
        }

        public int ModifiedBy { get; set; }

        [SpParam(Ignored = true)]
        public List<JobLevelStandardTemplateKPI> KPIIds { get; set; }

        public override bool IsValid()
        {
            return TemplateId > 0 && ModifiedBy > 0;
        }

        public override string GetSettingKey()
        {
            return "JobLevelStandard_Template_UpdateKPI";
        }
    }
}
