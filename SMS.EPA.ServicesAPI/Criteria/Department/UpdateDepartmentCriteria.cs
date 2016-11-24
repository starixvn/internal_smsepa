namespace SMS.EPA.ServicesAPI.Criteria.Department
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateDepartmentCriteria : CriteriaBase
    {
        private const string _DepartmentTpl = "<Department DepartmentId='{0}' Code='{1}' "
            + " Name='{2}' ParentId='{3}' ManagerId='{4}' DepartmentLevel='{5}' IsActive='{6}' ModifiedDate='{7}'></Department>";

        [SpParam(Ignored = true)]
        public IList<Model.Department> Departments { get; set; }

        public string DepartmentXml
        {
            get
            {
                var value = new StringBuilder("<ROOT>");

                foreach (Model.Department department in Departments)
                {
                    value.AppendFormat(
                        _DepartmentTpl,
                        department.DepartmentId,
                        department.Code,
                        HttpUtility.HtmlEncode(department.Name),
                        department.ParentId,
                        department.ManagerId,
                        department.DepartmentLevel,
                        !department.IsDeleted,
                        DateTime.Now);
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