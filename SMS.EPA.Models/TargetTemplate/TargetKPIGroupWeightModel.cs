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
        private const string _KPIGroup = "<KPIGroup KPIGroupId='{0}' Weight='{1}' IsSpreadKPI='{2}' DisplayOrder='{3}'></KPIGroup>";
        private const string _BeginRootTag = "<ROOT>";
        private const string _EndRootTag = "</ROOT>";

        public int TargetTemplateId { get; set; }

        public List<KPIGroupModel> KPIGroups { get; set; }

        public string ConvertKPIGroupWeightToXml
        {
            get
            {
                if (KPIGroups != null)
                {
                    var value = new StringBuilder(_BeginRootTag);
                    foreach (KPIGroupModel item in KPIGroups)
                    {
                        var spreadKPI = item.IsSpreadKPI == true ? 1 : 0;
                        value.AppendFormat(_KPIGroup, item.KPIGroupId, item.Weight, spreadKPI, item.DisplayOrder);
                    }

                    return value.Append(_EndRootTag).ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}
