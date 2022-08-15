namespace SMS.EPA.Model.JobLevelStandard.Period
{
    using System;
    using System.Xml.Serialization;
    using SMS.EPA.Core.Extensions;

    [Serializable]
    [XmlName("Row")]
    public class JobLevelStandardPeriodSetting
    {
        [XmlIgnore]
        public int PeriodId { get; set; }

        public byte ProcessId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}