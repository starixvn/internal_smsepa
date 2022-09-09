namespace SMS.EPA.Model.JobLevelStandard
{
    using System;

    [Serializable]
    public class JobLevelSettingSummary
    {
        public int SectionId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public int DepartmentId { get; set; }
        public string Department { get; set; }

        public int UnitId { get; set; }
        public string Unit { get; set; }

        public bool HasSetting { get; set; }
    }
}