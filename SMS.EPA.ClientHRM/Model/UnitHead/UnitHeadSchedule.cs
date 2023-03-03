namespace SMS.EPA.ClientHRM.Model.UnitHead
{
    using System;

    [Serializable]
    public class UnitHeadSchedule
    {
        public int EmpId { get; set; }
        public DateTime WorkDate { get; set; }
        public bool IsWorkDay { get; set; }
        public bool IsOffDay { get; set; }
        public bool IsHoliday { get; set; }
        public bool IsLeaveDay { get; set; }

        public string LeaveTypeCode { get; set; }
        public string LeaveTypeName { get; set; }
    }
}