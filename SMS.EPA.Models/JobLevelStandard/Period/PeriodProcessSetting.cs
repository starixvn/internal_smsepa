namespace SMS.EPA.Model.JobLevelStandard
{
    using System;
    using System.Xml.Serialization;
    using SMS.EPA.Core.Extensions;

    [Serializable]
    [XmlName("Row")]
    public class PeriodProcessSetting
    {
        public byte ProcessId { get; set; }
        public int EmployeeId { get; set; }

        public int EvaluatorId { get; set; }

        [XmlIgnore]
        public string Code { get; set; }

        [XmlIgnore]
        public string Nickname { get; set; }

        [XmlIgnore]
        public string FullName { get; set; }
        [XmlIgnore]
        public string JobLevel { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}