namespace SMS.EPA.Model
{
    using Core.Services;
    using Enitites;
    using System;

    [Serializable]
    public class EmployeeViewModel
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

        public string Category { get; set; }

        public DateTime JoinedDate { get; set; }

        public bool IsExistsSetting { get; set; }

        public bool HasMonthlyEPA { get; set; }

        public JobTitle JobTitle { get; set; }

        public DepartmentViewModel Department { get; set; }

        public Group Group { get; set; }
    }
}