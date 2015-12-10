// ----------------------------------------------------------------------
// <copyright file="TargetTemplateModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/10/2015 11:28:37 AM">Created and clone from NexEPA.</change>
// </history>

namespace SMS.EPA.Model.TargetTemplate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text;

    [Serializable]
    public class TargetTemplateModel
    {
        private const string KPIXmlTemplate = "<KPI KPIId='{0}' KPIGroupId='{1}' IsSpreadKPI='{2}' Weight='{3}' DisplayOrder='{4}'></KPI>";
        private const string BeginRootTag = "<ROOT>";
        private const string EndRootTag = "</ROOT>";

        public int TargetTemplateId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public byte? TargetTypeId { get; set; }

        public string TargetTypeName { get; set; }

        public int ScoreRatingTypeId { get; set; }

        public int RatingTypeId { get; set; }

        public string RatingTypeName { get; set; }

        public string ScoreRatingTypeName { get; set; }

        public int MaxWeightSelf { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public bool IsActive { get; set; }

        public int KPIs { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public List<TargetTemplateKPIModel> KPIIds { get; set; }

        public string CreatedNickName { get; set; }

        public string CreatedFullName { get; set; }

        public string ModifiedNickName { get; set; }

        public string ModifiedFullName { get; set; }

        public string ConvertKPIItemToXml(List<TargetTemplateKPIModel> kpiItems)
        {
            var kpiXml = new StringBuilder(BeginRootTag);

            if (kpiItems != null)
            {
                foreach (TargetTemplateKPIModel item in kpiItems)
                {
                    kpiXml.AppendFormat(KPIXmlTemplate, item.KPIId, item.KPIGroupId, item.IsSpreadKPI, item.Weight, item.DisplayOrder);
                }
            }

            return kpiXml.Append(EndRootTag).ToString();
        }
    }
}
