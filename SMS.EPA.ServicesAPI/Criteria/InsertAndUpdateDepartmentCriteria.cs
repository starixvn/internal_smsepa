namespace SMS.EPA.ServicesAPI.Criteria
{
    using Fanex.Data.Repository;
    using SMS.EPA.Model.Department;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;

    public class InsertAndUpdateDepartmentCriteria: CriteriaBase
    {
        private const string _DepartmentTpl = "<Department DepartmentId='{0}' Code='{1}' "
            + "Name='{2}' ParentId='{3}' ManagerId='{4}' DepartmentLevel='{5}' IsActive='{6}' ModifiedDate='{7}'></Department>";

        [SpParam(Ignored = true)]
        public IList<DepartmentModel> Departments { get; set; }

        public string DepartmentXml
        {
            get
            {
                var value = new StringBuilder("<ROOT>");

                if (Departments != null)
                {
                    foreach (DepartmentModel department in Departments)
                    {
                        value.AppendFormat(
                            _DepartmentTpl,
                            department.DepartmentId,
                            department.Code,
                            HttpUtility.HtmlEncode(department.Name),
                            department.ParentId,
                            department.Manager.EmployeeId,
                            '1',
                            department.IsActive,
                            DateTime.Now);
                    }
                }

                return value.Append("</ROOT>").ToString();
            }
        }

        public override string GetSettingKey()
        {
            return "Sync_Department";
        }

        public override bool IsValid()
        {
            return Departments != null && Departments.Any();
        }
    }
}