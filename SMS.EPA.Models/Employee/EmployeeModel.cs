namespace SMS.EPA.Model.Employee
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.Enitites;

    [Serializable]
    public class EmployeeModel : Employee
    {
        public EmployeeModel()
        {
            Roles = new List<SystemRoleModel>();
        }

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

        public DepartmentModel Department { get; set; }

        public Group Group { get; set; }

        public IList<SystemRoleModel> Roles { get; set; }
    }
}