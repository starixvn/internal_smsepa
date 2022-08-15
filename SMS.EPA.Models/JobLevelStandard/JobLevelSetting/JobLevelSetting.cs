namespace SMS.EPA.Model.JobLevelStandard
{
    using System;
    using System.Xml.Serialization;
    using SMS.EPA.Core.Extensions;

    [Serializable]
    [XmlName("Row")]
    public class JobLevelSetting
    {
        public int JobLevelId { get; set; }

        [XmlIgnore]
        public string Name { get; set; }

        public decimal? MinWeight { get; set; }
        public decimal? MaxWeight { get; set; }
    }
}