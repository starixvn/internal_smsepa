namespace SMS.EPA.Model
{
    using SMS.EPA.Core.Services;

    public class EvaluatorInfoViewModel
    {
        public int EmployeeId { get; set; }

        public string Code { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Avatar
        {
            get
            {
                IUtilityService utilityService = new UtilityService();
                return utilityService.GetAvatar(Code);
            }
        }

        public bool IsExistsSetting { get; set; }

        public bool HasMonthlyEPA { get; set; }
    }
}