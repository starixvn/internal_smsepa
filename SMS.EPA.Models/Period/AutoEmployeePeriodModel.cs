namespace SMS.EPA.Model
{
    using SMS.EPA.Core.Enumerations;

    public class AutoEmployeePeriodModel
    {
        public int EPAId { get; set; }

        public string EmployeeIds { get; set; }

        public int PeriodId { get; set; }

        public int TargetTypeId { get; set; }

        public int ProcessId { get; set; }

        public string TotalScore { get; set; }

        public int EmployeeId { get; set; }

        public bool HasMonthlyEPA { get; set; }

        public bool IsSemiHasMonthly
        {
            get
            {
                return TargetTypeId == (int)TargetType.SemiAnnual && HasMonthlyEPA;
            }
        }
    }
}