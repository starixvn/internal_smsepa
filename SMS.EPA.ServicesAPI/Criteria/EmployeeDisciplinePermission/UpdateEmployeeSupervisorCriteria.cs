namespace SMS.EPA.ServicesAPI.Criteria.EmployeeDisciplinePermission
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.ServicesAPI.Model;

    public class UpdateEmployeeSupervisorCriteria : CriteriaBase
    {
        private const string _SupervisorTpl =
             "<Supervisor OwnerId='{0}' EmployeeId='{1}' ModifiedDate='{2}' IsDeleted='{3}'></Supervisor>";

        [SpParam(Ignored = true)]
        public IList<EmployeeDisciplinePermission> EmployeeDisciplines { get; set; }

        public string SupervisorXml
        {
            get
            {
                var value = new StringBuilder("<ROOT>");

                if (EmployeeDisciplines != null)
                {
                    foreach (EmployeeDisciplinePermission supervisor in EmployeeDisciplines)
                    {
                        value.AppendFormat(
                            _SupervisorTpl,
                            supervisor.OwnerId,
                            supervisor.EmployeeId,
                            supervisor.ModifiedDate,
                            supervisor.IsDeleted);
                    }
                }

                return value.Append("</ROOT>").ToString();
            }
        }

        public override string GetSettingKey()
        {
            return "Sync_EmployeeDisciplinePermission";
        }

        public override bool IsValid()
        {
            return EmployeeDisciplines != null && EmployeeDisciplines.Any();
        }
    }
}