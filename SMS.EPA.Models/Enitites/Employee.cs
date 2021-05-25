namespace SMS.EPA.Model.Enitites
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Constants;
    using Core.Model;
    using Core.Services;
    using Report;

    [Serializable]
    public class Employee : IExcelRecord
    {
        public Employee()
        {
            Roles = new List<SystemRoleModel>();
        }

        public int EmployeeId { get; set; }

        public string Code { get; set; }

        public string DomainUsername { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string Password { get; set; }

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

        public bool Enabled { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsLocal { get; set; }

        public DateTime JoinedDate { get; set; }

        public bool HasMonthlyEPA { get; set; }

        public string Username => !string.IsNullOrWhiteSpace(DomainUsername) && IsSpecialUser
            ? DomainUsername.Replace("sms\\", string.Empty)
            : string.Empty;

        public bool IsSpecialUser { get; set; }

        public bool IsExistsSetting { get; set; }

        public JobTitle JobTitle { get; set; }

        public JobLevel JobLevel { get; set; }

        public Department Department { get; set; }

        public Group Group { get; set; }

        public IList<SystemRoleModel> Roles { get; set; }

        public object ConvertToExcelRecord()
        {
            var record = new
            {
                Count = 0,
                Code,
                Username,
                NickName,
                FullName,
                Department = Department != null ? Department.Name : string.Empty,
                Section = Group != null ? Group.GroupName : string.Empty,
                JobTitle = JobTitle.JobTitleName,
                JobLevel = JobLevel.Name,
                Category,
                JoinedDate = JoinedDate.ToString(Constants.DateFormat),
                Email,
                Roles = string.Join(", ", Roles.Select(x => x.RoleCode))
            };

            return record;
        }
    }
}