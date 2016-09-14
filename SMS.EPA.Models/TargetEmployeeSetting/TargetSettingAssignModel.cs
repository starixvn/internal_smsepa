namespace SMS.EPA.Model.TargetEmployeeSetting
{
    using System.Collections.Generic;
    using System.Text;
    using SMS.EPA.Model.Enitites;

    public class TargetSettingAssignModel
    {
        private const string DepartmentXmlTemplate = "<Department DepartmentId='{0}' IsGroup='{1}'></Department>";
        private const string BeginRootTag = "<ROOT>";
        private const string EndRootTag = "</ROOT>";

        public int TargetTemplateId { get; set; }

        public IEnumerable<Department> DepartmentSettings { get; set; }

        public string ConvertToXml()
        {
            var departmentXml = new StringBuilder(BeginRootTag);

            if (DepartmentSettings != null)
            {
                foreach (Department department in DepartmentSettings)
                {
                    departmentXml.AppendFormat(DepartmentXmlTemplate, department.DepartmentId, department.IsGroup ? 1 : 0);
                }
            }

            departmentXml.Append(EndRootTag);
            return departmentXml.ToString();
        }
    }
}