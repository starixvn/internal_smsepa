namespace SMS.EPA.Criteria
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;

    public class UpdateEPASummaryImportCriteria : CriteriaBase
    {
        private const string EPAXmlTemplate = "<EPA Id='{0}' FinalScore='{1}'></EPA>";

        [SpParam(Ignored = true)]
        public IEnumerable<EPASummaryImportModel> EpaInfos { get; set; }

        public string EPADetailXml
        {
            get
            {
                var epaXmlBuilder = new StringBuilder("<ROOT>");

                if (EpaInfos != null && EpaInfos.Any())
                {
                    foreach (EPASummaryImportModel epa in EpaInfos)
                    {
                        epaXmlBuilder.AppendFormat(
                            EPAXmlTemplate,
                            epa.Id,
                            epa.FinalScore);
                    }
                }

                epaXmlBuilder.Append("</ROOT>");
                return epaXmlBuilder.ToString();
            }
        }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(EPADetailXml);
        }

        public override string GetSettingKey()
        {
            return "EPASummaryImport_Update";
        }
    }
}