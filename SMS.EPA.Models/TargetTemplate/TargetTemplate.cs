namespace SMS.EPA.Model
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class TargetTemplate
    {
        public int TargetTemplateId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public byte TargetTypeId { get; set; }

        public string TargetTypeName { get; set; }

        public byte ScoreRatingTypeId { get; set; }

        public byte RatingTypeId { get; set; }

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

        public string Teams { get; set; }
    }
}