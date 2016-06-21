namespace SMS.EPA.Model.AttendanceSummary
{
    using System;

    [Serializable]
    public class ViolationModel
    {
        public int ViolationId { get; set; }

        public string Name { get; set; }

        public float Score { get; set; }

        public string Value { get; set; }

        public bool HasViolated { get; set; }

        public bool HasIgnored { get; set; }

        public bool HasApprovedBy { get; set; }
    }
}