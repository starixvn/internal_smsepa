namespace SMS.EPA.Model
{
    using Core.Enumerations;

    public class AutoEmployeePeriodModel
    {
        public int EPAId { get; set; }

        public string EmployeeIds { get; set; }

        public int PeriodId { get; set; }

        public byte TargetTypeId { get; set; }

        public byte ProcessId { get; set; }

        public string TotalScore { get; set; }

        public int EmployeeId { get; set; }

        public bool HasMonthlyEPA { get; set; }

        public bool IsSemiHasMonthly => TargetTypeId == (byte)TargetType.SemiAnnual && HasMonthlyEPA;
    }
}