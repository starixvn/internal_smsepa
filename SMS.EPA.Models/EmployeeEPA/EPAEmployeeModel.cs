namespace SMS.EPA.Model
{
    using System;
    using SMS.EPA.Core.Services;

    [Serializable]
    public class EPAEmployeeModel
    {
        public int EPAId { get; set; }

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

        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public string GroupCode { get; set; }

        public int DepartmentId { get; set; }

        public string DepartmentCode { get; set; }

        public string DepartmentName { get; set; }

        public int JobTitleId { get; set; }

        public string JobTitleName { get; set; }

        public DateTime JoinedDate { get; set; }
    }
}