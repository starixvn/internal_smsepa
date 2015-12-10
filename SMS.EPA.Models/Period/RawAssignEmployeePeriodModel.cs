// ----------------------------------------------------------------------
// <copyright file="RawAssignEmployeePeriodModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/13/2015">Created.</change>
// </history>

namespace SMS.EPA.Model.Period
{
    using System.Collections.Generic;
    using System.Text;
    using SMS.EPA.Model.Department;

    public class RawAssignEmployeePeriodModel
    {
        private const string DepartmentXmlTemplate = "<Department DepartmentId='{0}' IsGroup='{1}'></Department>";
        private const string BeginRootTag = "<ROOT>";
        private const string EndRootTag = "</ROOT>";

        public int PeriodId { get; set; }

        public IEnumerable<DepartmentModel> Departments { get; set; }

        public int? EmployeeId { get; set; }

        public string ConvertToXml()
        {
            var departmentXml = new StringBuilder(BeginRootTag);

            if (Departments != null)
            {
                foreach (DepartmentModel department in Departments)
                {
                    departmentXml.AppendFormat(DepartmentXmlTemplate, department.DepartmentId, department.IsGroup ? 1 : 0);
                }
            }

            departmentXml.Append(EndRootTag);
            return departmentXml.ToString();
        }
    }
}
