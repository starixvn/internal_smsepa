namespace SMS.EPA.Model.Enitites
{
    using SMS.EPA.Core.Model;
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class Employee
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
                return SMS.EPA.Core.Utilities.Avatar.GetAvatar(Code);
            }
        }

        public string Category { get; set; }

        public bool Enabled { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsLocal { get; set; }

        public DateTime JoinedDate { get; set; }

        public bool HasMonthlyEPA { get; set; }

        public string Username
        {
            get
            {
                return !string.IsNullOrWhiteSpace(DomainUsername) && IsSpecialUser ? DomainUsername.Replace("sms\\", string.Empty) : string.Empty;
            }
        }

        public bool IsSpecialUser { get; set; }

        public bool IsExistsSetting { get; set; }

        public JobTitle JobTitle { get; set; }

        public Department Department { get; set; }

        public Group Group { get; set; }

        public IList<SystemRoleModel> Roles { get; set; }
    }
}