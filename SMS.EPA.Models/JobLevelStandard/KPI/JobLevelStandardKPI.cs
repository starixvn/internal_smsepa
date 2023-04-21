namespace SMS.EPA.Model.JobLevelStandard
{
    using System;

    [Serializable]
    public class JobLevelStandardKPI
    {
        public int KPIId { get; set; }
        public string Name { get; set; }
        public byte DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}