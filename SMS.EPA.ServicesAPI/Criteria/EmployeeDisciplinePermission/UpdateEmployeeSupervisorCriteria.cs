namespace SMS.EPA.ServicesAPI.Criteria.EmployeeDisciplinePermission
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Core.Constants;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model.API;

    public class UpdateEmployeeSupervisorCriteria : CriteriaBase
    {
        private const string SupervisorTpl =
             "<Supervisor OwnerId='{0}' EmployeeId='{1}' ModifiedDate='{2}' IsDeleted='{3}'></Supervisor>";

        [SpParam(Ignored = true)]
        public IList<EmployeeDisciplinePermission> EmployeeDisciplines { get; set; }

        public string SupervisorXml
        {
            get
            {
                var value = new StringBuilder(Constants.BeginRootTag);

                if (EmployeeDisciplines != null)
                {
                    foreach (EmployeeDisciplinePermission supervisor in EmployeeDisciplines)
                    {
                        value.AppendFormat(
                            SupervisorTpl,
                            supervisor.OwnerId,
                            supervisor.EmployeeId,
                            supervisor.ModifiedDate.ToString(Constants.ModifiedDateTimeFormat),
                            supervisor.IsDeleted);
                    }
                }

                return value.Append(Constants.EndRootTag).ToString();
            }
        }

        public override string GetSettingKey() => "Sync_EmployeeDisciplinePermission";

        public override bool IsValid() => EmployeeDisciplines != null && EmployeeDisciplines.Any();
    }
}