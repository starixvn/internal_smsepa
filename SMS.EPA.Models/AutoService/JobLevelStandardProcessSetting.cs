namespace SMS.EPA.Model.AutoService
{
    using System;

    public class JobLevelStandardProcessSetting
    {
        public int PeriodId { get; set; }
        public string PeriodName { get; set; }

        public byte ProcessId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public int EvaluatorId { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}