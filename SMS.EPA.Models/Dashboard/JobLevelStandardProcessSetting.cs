namespace SMS.EPA.Model.Dashboard
{
    using System;

    public class JobLevelStandardProcessSetting
    {
        public int PeriodId { get; set; }
        public string PeriodName { get; set; }

        public byte ProcessId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}