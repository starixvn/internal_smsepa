namespace SMS.EPA.ServicesAPI.Criteria.Department
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;
    using Core.Constants;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model.API;

    public class UpdateDepartmentCriteria : CriteriaBase
    {
        private const string DepartmentTpl = "<Department DepartmentId='{0}' Code='{1}' "
            + " Name='{2}' ParentId='{3}' ManagerId='{4}' DepartmentLevel='{5}' IsActive='{6}' ModifiedDate='{7}'></Department>";

        [SpParam(Ignored = true)]
        public IList<Department> Departments { get; set; }

        public string DepartmentXml
        {
            get
            {
                var value = new StringBuilder(Constants.BeginRootTag);

                foreach (Department department in Departments)
                {
                    value.AppendFormat(
                        DepartmentTpl,
                        department.DepartmentId,
                        HttpUtility.HtmlEncode(department.Code),
                        HttpUtility.HtmlEncode(department.Name),
                        department.ParentId,
                        department.ManagerId,
                        department.DepartmentLevel,
                        !department.IsDeleted,
                        department.ModifiedDate.ToString(Constants.ModifiedDateTimeFormat));
                }

                return value.Append(Constants.EndRootTag).ToString();
            }
        }

        public override string GetSettingKey() => "Sync_Department";

        public override bool IsValid() => Departments != null && Departments.Any();
    }
}