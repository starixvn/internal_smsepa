namespace SMS.EPA.Model.JobLevelStandard.Template
{
    using System;

    public class JobLevelStandardTemplate
    {
        public int TemplateId { get; set; }

        public string Name { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public bool IsActive { get; set; }

        public int KPIs { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public string CreatedNickName { get; set; }

        public string CreatedFullName { get; set; }

        public string ModifiedNickName { get; set; }

        public string ModifiedFullName { get; set; }
    }
}