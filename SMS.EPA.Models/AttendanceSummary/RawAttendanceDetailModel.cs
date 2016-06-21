namespace SMS.EPA.Model.AttendanceSummary
{
    using System.Collections.Generic;
    using SMS.EPA.Model.Employee;

    public class RawAttendanceDetailModel
    {
        public RawAttendanceDetailModel()
        {
            Employees = new List<EmployeeModel>();
            Violations = new List<AttendanceDetailModel>();
        }

        public IEnumerable<EmployeeModel> Employees { get; set; }

        public IEnumerable<AttendanceDetailModel> Violations { get; set; }

        public IEnumerable<AttendanceDetailModel> CommentViolations { get; set; }
    }
}