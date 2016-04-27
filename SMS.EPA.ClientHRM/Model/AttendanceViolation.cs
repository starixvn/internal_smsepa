using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.EPA.ServicesAPI.Model
{
    public class AttendanceViolation
    {
        public int EmployeeId { get; set; }

        public DateTime WorkDate { get; set; }

        public string ShiftName { get; set; }

        public DateTime CheckIn { get; set; }

        public DateTime CheckOut { get; set; }

        public float TotalHours { get; set; }

        public int LateIn { get; set; }

        public int EarlyOut { get; set; }

        public int Type { get; set; }

        public float DisciplineScore { get; set; }

        public string EmployeeComment { get; set; }

        public string ApproverComment { get; set; }

        public bool IsIgnored { get; set; }

        public DateTime ModifiedDate { get; set; }

        public bool IsDeleted { get; set; }
    }
}
