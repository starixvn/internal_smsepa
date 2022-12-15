namespace SMS.EPA.Model.Enitites
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HRM.UserControl.Client.Core.Models.Role;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Model.Report;

    [Serializable]
    public class Employee : IExcelRecord
    {
        public int EmployeeId { get; set; }
        public int UserId { get; set; }

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

        public string Username => !string.IsNullOrWhiteSpace(DomainUsername)
            ? DomainUsername.Replace("sms\\", string.Empty) : string.Empty;

        public bool IsSpecialUser { get; set; }

        public bool IsExistsSetting { get; set; }

        public JobTitle JobTitle { get; set; }

        public JobLevel JobLevel { get; set; }

        public Department Department { get; set; }

        public Group Group { get; set; }

        public IList<Role> Roles { get; set; } = new List<Role>();

        public object ConvertToExcelRecord()
        {
            var record = new
            {
                Count = 0,
                Code,
                Username,
                NickName,
                FullName,
                Section = Department?.Name,
                Team = Group?.GroupName,
                JobTitle = JobTitle?.JobTitleName,
                JobLevel = JobLevel?.Name,
                Category,
                JoinedDate = JoinedDate.ToString(Constants.DateFormat),
                Email,
                Roles = string.Join(", ", Roles.Select(x => x.Name))
            };

            return record;
        }
    }
}