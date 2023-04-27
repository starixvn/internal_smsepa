namespace SMS.EPA.Model.JobLevelStandard
{
    using System;

    [Serializable]
    public class JobLevelStandardKPI
    {
        public int KPIId { get; set; }
        public string Name { get; set; }
        public int? DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public bool IsSelected { get; set; }
    }
}