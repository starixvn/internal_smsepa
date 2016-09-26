namespace SMS.EPA.Model.Period
{
    using SMS.EPA.Core.Services;

    public class AutoEmployeePeriodModel
    {
        public int EPAId { get; set; }

        public string EmployeeIds { get; set; }

        public int PeriodId { get; set; }

        public int TargetTypeId { get; set; }

        public int ProcessId { get; set; }

        public string TotalScoreEncrypt { get; set; }

        public string TotalScore
        {
            get
            {
                return CryptographyService.Decrypt(TotalScoreEncrypt);
            }
        }

        public int EmployeeId { get; set; }

        public bool HasMonthlyEPA { get; set; }
    }
}