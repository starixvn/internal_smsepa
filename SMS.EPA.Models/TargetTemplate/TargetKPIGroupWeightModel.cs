// ----------------------------------------------------------------------
// <copyright file="TargetKPIGroupWeightModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/13/2015 9:58:53 AM">Created.</change>
// </history>

namespace SMS.EPA.Model.TargetTemplate
{
    using System.Collections.Generic;
    using System.Text;
    using SMS.EPA.Model.KPIGroup;

    public class TargetKPIGroupWeightModel
    {
        private const string KPIGroupXmlTemplate = "<KPIGroup KPIGroupId='{0}' Weight='{1}' IsSpreadKPI='{2}' DisplayOrder='{3}'></KPIGroup>";
        private const string BeginRootTag = "<ROOT>";
        private const string EndRootTag = "</ROOT>";

        public int TargetTemplateId { get; set; }

        public List<KPIGroupModel> KPIGroups { get; set; }

        public string ConvertKPIGroupWeightToXml
        {
            get
            {
                string kpiGroupXml = string.Empty;

                if (KPIGroups != null)
                {
                    var kpiGroupXmlBuilder = new StringBuilder(BeginRootTag);

                    foreach (KPIGroupModel item in KPIGroups)
                    {
                       int spreadKPI = item.IsSpreadKPI == true ? 1 : 0;
                        kpiGroupXmlBuilder.AppendFormat(KPIGroupXmlTemplate, item.KPIGroupId, item.Weight, spreadKPI, item.DisplayOrder);
                    }

                    kpiGroupXml = kpiGroupXmlBuilder.Append(EndRootTag).ToString();
                }

                return kpiGroupXml;
            }
        }
    }
}
