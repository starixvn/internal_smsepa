namespace SMS.EPA.Model.JobLevelStandard
{
    using System;

    [Serializable]
    public class JobLevelSettingSummary
    {
        public int SectionId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string Unit { get; set; }
        public bool HasSetting { get; set; }
    }
}