namespace SMS.EPA.Criteria
{
    using System.Collections.Generic;
    using System.Text;
    using Fanex.Data.Repository;
    using SMS.EPA.Model;

    public class UpdateEPASummaryImportCriteria : CriteriaBase
    {
        private const string EPAXmlTemplate = "<EPA Id='{0}' FinalScore='{1}'></EPA>";

        [SpParam(Ignored = true)]
        public IEnumerable<EPASummaryImportModel> MemberEPAs { get; set; }

        public string EPADetailXML
        {
            get
            {
                var epaXmlBuilder = new StringBuilder("<ROOT>");

                if (MemberEPAs != null)
                {
                    foreach (EPASummaryImportModel epa in MemberEPAs)
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
            return !string.IsNullOrWhiteSpace(EPADetailXML);
        }

        public override string GetSettingKey()
        {
            return "EPASummaryImport_Update";
        }
    }
}