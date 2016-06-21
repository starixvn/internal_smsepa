namespace SMS.EPA.Model.EmployeeEPA
{
    using System;

    public class EmployeeOtherViolationDetailModel
    {
        public int EPAId { get; set; }

        public DateTime WorkDate { get; set; }

        public string Name { get; set; }

        public float Score { get; set; }

        public string Comment { get; set; }
    }
}