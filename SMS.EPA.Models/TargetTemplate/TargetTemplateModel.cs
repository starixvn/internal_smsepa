namespace SMS.EPA.Model.TargetTemplate
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    [Serializable]
    public class TargetTemplateModel
    {
        private const string KPIXmlTemplate = @"<KPI KPIId='{0}' KPIGroupId='{1}' IsSpreadKPI='{2}' Weight='{3}' DisplayOrder='{4}'
                IsRequiredEvaluate='{5}' HasComment='{6}' IsRequiredComment='{7}'></KPI>";

        private const string BeginRootTag = "<ROOT>";
        private const string EndRootTag = "</ROOT>";

        public int TargetTemplateId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int TargetTypeId { get; set; }

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