namespace SMS.EPA.ServicesAPI.Criteria.Employee
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.ServicesAPI.Model;

    public class UpdateEmployeeCriteria : CriteriaBase
    {
        private const string _UserTpl =
            "<User EmployeeId='{0}' DomainUsername='{1}' Code='{2}' "
            + "NickName='{3}' FullName='{4}' TeamId='{5}' JobTitleId='{6}' Phone='{7}' "
            + " Email='{8}' CardNumber='{9}' IsEnabled='{10}' IsDeleted='{11}' Password='{12}' "
            + " JoinedDate='{13}' ModifiedDate='{14}' IsLocal='{15}' ResignedDate ='{16}' ConfirmedDate='{17}'></User>";

        [SpParam(Ignored = true)]
        public IList<Employee> Employees { get; set; }

        public string UserXml
        {
            get
            {
                var value = new StringBuilder("<ROOT>");

                if (Employees != null)
                {
                    foreach (Employee employee in Employees)
                    {
                        value.AppendFormat(
                            _UserTpl,
                            employee.EmployeeId,
                            employee.DomainUsername,
                            employee.Code,
                            HttpUtility.HtmlEncode(employee.NickName),
                            HttpUtility.HtmlEncode(employee.FullName),
                            employee.TeamId,
                            employee.JobTitleId,
                            HttpUtility.HtmlEncode(employee.Phone),
                            HttpUtility.HtmlEncode(employee.Email),
                            employee.EmployeeCard,
                            employee.IsEnabled,
                            employee.IsDeleted,
                            employee.Password,
                            employee.JoinedDate,
                            employee.ModifiedDate,
                            employee.IsLocal,
                            employee.ResignedDate,
                            employee.ConfirmedDate);
                    }
                }

                return value.Append("</ROOT>").ToString();
            }
        }

        public override string GetSettingKey()
        {
            return "Sync_Employee";
        }

        public override bool IsValid()
        {
            return Employees != null && Employees.Any();
        }
    }
}