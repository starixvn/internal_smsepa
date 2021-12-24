namespace SMS.EPA.Core.Model
{
    using System;
    [Serializable]
    public class UserSession
    {
        public int EmployeeId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string Password { get; set; }

        public bool IsSpecialUser { get; set; }
    }
}