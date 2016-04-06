// -----------------------------------------------------------------------
// <copyright file="EPAEmployeeModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.09.15">Created</change>
// </history>

namespace SMS.EPA.Model.EmployeeEPA
{
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
                return SMS.EPA.Core.Utilities.Avatar.GetAvatar(Code);
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
    }
}