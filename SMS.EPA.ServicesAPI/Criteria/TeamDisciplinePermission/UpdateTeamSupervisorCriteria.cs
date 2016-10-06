namespace SMS.EPA.ServicesAPI.Criteria.TeamDisciplinePermission
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.ServicesAPI.Model;

    public class UpdateTeamDisciplinePermissionCriteria : CriteriaBase
    {
        private const string _SupervisorTpl =
             "<Supervisor OwnerId='{0}' GroupId='{1}' ModifiedDate='{2}' IsDeleted='{3}'></Supervisor>";

        [SpParam(Ignored = true)]
        public IList<TeamDisciplinePermission> EmployeeDisciplines { get; set; }

        public string SupervisorXml
        {
            get
            {
                var value = new StringBuilder("<ROOT>");

                if (EmployeeDisciplines != null)
                {
                    foreach (TeamDisciplinePermission supervisor in EmployeeDisciplines)
                    {
                        value.AppendFormat(
                            _SupervisorTpl,
                            supervisor.OwnerId,
                            supervisor.TeamId,
                            supervisor.ModifiedDate,
                            supervisor.IsDeleted);
                    }
                }

                return value.Append("</ROOT>").ToString();
            }
        }

        public override string GetSettingKey()
        {
            return "Sync_TeamDisciplinePermission";
        }

        public override bool IsValid()
        {
            return EmployeeDisciplines != null && EmployeeDisciplines.Any();
        }
    }
}