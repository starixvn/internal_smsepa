namespace SMS.EPA.Criteria
{
    using System.Collections.Generic;
    using System.Text;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Model.Enitites;

    public class AddEmployeeToPeriodCriteria : CriteriaBase
    {
        private const string DepartmentXmlTemplate = "<Department DepartmentId='{0}' IsGroup='{1}'></Department>";

        public int PeriodId { get; set; }

        [SpParam(SqlParamName = "Departments")]
        public string DepartmentXml
        {
            get
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

        [SpParam(Ignored = true)]
        public IEnumerable<Department> Departments { get; set; }

        public int? EmployeeId { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return PeriodId > 0 && ModifiedBy > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaPeriod_AddEmployee";
        }
    }
}