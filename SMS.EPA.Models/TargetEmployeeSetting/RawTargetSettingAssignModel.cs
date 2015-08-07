// ----------------------------------------------------------------------
// <copyright file="RawTargetSettingAssignModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/7/2015">Created.</change>
// </history>

namespace SMS.EPA.Model.TargetEmployeeSetting
{
    using System.Collections.Generic;
    using System.Text;
    using SMS.EPA.Model.Department;

    public class RawTargetSettingAssignModel
    {
        private const string _DepartmentItem = "<Department DepartmentId='{0}' IsGroup='{1}'></Department>";
        private const string _BeginRootTag = "<ROOT>";
        private const string _EndRootTag = "</ROOT>";

        public int TargetTemplateId { get; set; }

        public IEnumerable<DepartmentModel> DepartmentSettings { get; set; }

        public string ConvertToXml() 
        {
            var value = new StringBuilder(_BeginRootTag);

            if (DepartmentSettings != null)
            {
                foreach (DepartmentModel department in DepartmentSettings)
                {
                    value.AppendFormat(_DepartmentItem, department.DepartmentId, department.IsGroup ? 1 : 0);
                }
            }

            value.Append(_EndRootTag);
            return value.ToString();
        }
    }
}
