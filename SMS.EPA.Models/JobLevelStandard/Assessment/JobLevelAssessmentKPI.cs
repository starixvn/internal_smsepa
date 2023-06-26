namespace SMS.EPA.Model.JobLevelStandard
{
    using System;
    using System.Xml.Serialization;
    using SMS.EPA.Core.Extensions;

    [Serializable]
    [XmlName("Row")]
    public class JobLevelAssessmentKPI
    {
        [XmlIgnore]
        public int EmployeeId { get; set; }

        [XmlIgnore]
        public int? AssessmentId { get; set; }
        public string Template { get; set; }
        public int KPIId { get; set; }
        public string KPIName { get; set; }
        public decimal? TemplateWeight { get; set; }
        public byte? Level { get; set; }
        public decimal? Weight { get; set; }
        public byte? OldLevel { get; set; }
        public decimal? OldWeight { get; set; }
    }
}