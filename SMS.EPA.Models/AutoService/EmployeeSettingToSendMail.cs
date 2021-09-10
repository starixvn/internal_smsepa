namespace SMS.EPA.Model.AutoService
{
    using System;

    public class EmployeeSettingToSendMail
    {
        public int PeriodId { get; set; }
        public string Period { get; set; }

        public byte ProcessId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool HasSendMail { get; set; }

        public int EvaluatorId { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}