namespace SMS.EPA.Model.Enitites
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
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
        public string Section { get; set; }

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

        public string Username
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(DomainUsername))
                {
                    var username = DomainUsername;
                    if (DomainUsername.IndexOf("sms\\", StringComparison.OrdinalIgnoreCase) != -1)
                    {
                        username = ReplaceString(DomainUsername, "sms\\", string.Empty, StringComparison.OrdinalIgnoreCase);
                    }

                    return username;
                }

                return DomainUsername;
            }
        }

        public bool IsSpecialUser { get; set; }

        public bool IsExistsSetting { get; set; }

        public JobTitle JobTitle { get; set; }

        public JobLevel JobLevel { get; set; }

        public string Department { get; set; }

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
                Department,
                Section,
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

        private static string ReplaceString(string str, string oldValue, string newValue, StringComparison comparison)
        {
            if (string.IsNullOrEmpty(oldValue))
            {
                return str;
            }

            var sb = new StringBuilder();

            int previousIndex = 0;
            int index = str.IndexOf(oldValue, comparison);

            while (index != -1)
            {
                sb.Append(str.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = str.IndexOf(oldValue, index, comparison);
            }

            sb.Append(str.Substring(previousIndex));

            return sb.ToString();
        }
    }
}