namespace SMS.EPA.Core.Model
{
    using System.Collections.Generic;

    public class UserSession
    {
        public UserSession()
        {
            Roles = new List<SystemRoleModel>();
            Functions = new List<FunctionModel>();
        }

        public int EmployeeId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string Password { get; set; }

        public bool IsSpecialUser { get; set; }

        public IEnumerable<SystemRoleModel> Roles { get; set; }

        public IEnumerable<FunctionModel> Functions { get; set; }
    }
}