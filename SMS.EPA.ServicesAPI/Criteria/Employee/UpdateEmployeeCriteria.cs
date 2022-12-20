namespace SMS.EPA.ServicesAPI.Criteria.Employee
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Security;
    using Core.Services;
    using EPA.Model.API;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateEmployeeCriteria : CriteriaBase
    {
        private const string UserTpl = "<Row EmployeeId='{0}' DomainUsername='{1}' Code='{2}'"
            + " NickName='{3}' FullName='{4}' TeamId='{5}' JobTitleId='{6}' Phone='{7}'"
            + " Email='{8}' CardNumber='{9}' IsEnabled='{10}' IsDeleted='{11}' Password='{12}' JoinedDate='{13}' ModifiedDate='{14}'"
            + " IsLocal='{15}' ResignedDate ='{16}' ConfirmedDate='{17}' JobLevelId='{18}' UserId='{19}' MilitaryFromDate='{20}' MilitaryToDate='{21}'/>";

        [SpParam(Ignored = true)]
        public IList<Employee> Employees { get; set; }

        public string UserXml
        {
            get
            {
                var value = new StringBuilder(Constants.BeginRootTag);
                var encryptedPassword = bool.Parse(ConfigurationManagerService.GetAppSetting("EncryptedPassword"));

                if (Employees != null && Employees.Any())
                {
                    foreach (Employee employee in Employees)
                    {
                        if (!encryptedPassword)
                        {
                            employee.Password = Encryption.ComputeHash(HashProvider.Sha1, employee.Password);
                        }

                        value.AppendFormat(
                            UserTpl,
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
                            FormatDateTime(employee.JoinedDate),
                            FormatDateTime(employee.ModifiedDate),
                            employee.IsLocal,
                            FormatDateTime(employee.ResignedDate),
                            FormatDateTime(employee.ConfirmedDate),
                            employee.JobLevelId,
                            employee.UserId,
                            FormatDateTime(employee.MilitaryFromDate),
                            FormatDateTime(employee.MilitaryToDate));
                    }
                }

                return value.Append(Constants.EndRootTag).ToString();
            }
        }

        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }

        public override string GetSettingKey() => "Sync_Employee";

        public override bool IsValid() => Employees != null && Employees.Any();

        private static string FormatDateTime(DateTime? date)
            => date.HasValue ? date.Value.ToString(Constants.ModifiedDateTimeFormat) : string.Empty;
    }
}