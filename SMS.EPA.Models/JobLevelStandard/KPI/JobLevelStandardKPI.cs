namespace SMS.EPA.Model.JobLevelStandard
{
    using System;

    [Serializable]
    public class JobLevelStandardKPI
    {
        public int KPIId { get; set; }
        public string Name { get; set; }
        public byte DisplayOrder { get; set; }
    }
}