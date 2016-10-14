namespace SMS.EPA.Model.Period
{
    using System.Collections.Generic;
    using System.Text;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Model.Enitites;

    public class AssignEmployeePeriodModel
    {
        private const string DepartmentXmlTemplate = "<Department DepartmentId='{0}' IsGroup='{1}'></Department>";

        public int PeriodId { get; set; }

        public IEnumerable<Department> Departments { get; set; }

        public int? EmployeeId { get; set; }

        public string ConvertToXml()
        {
            var departmentXml = new StringBuilder(Constants.BeginRootTag);

            if (Departments != null)
            {
                foreach (Department department in Departments)
                {
                    departmentXml.AppendFormat(DepartmentXmlTemplate, department.DepartmentId, department.IsGroup ? 1 : 0);
                }
            }

            return departmentXml.Append(Constants.EndRootTag).ToString();
        }
    }
}