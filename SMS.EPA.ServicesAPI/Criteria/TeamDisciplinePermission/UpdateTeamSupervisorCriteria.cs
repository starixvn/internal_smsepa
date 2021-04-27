namespace SMS.EPA.ServicesAPI.Criteria.TeamDisciplinePermission
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Core.Constants;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model.API;

    public class UpdateTeamDisciplinePermissionCriteria : CriteriaBase
    {
        private const string SupervisorTpl =
             "<Supervisor OwnerId='{0}' GroupId='{1}' ModifiedDate='{2}' IsDeleted='{3}'></Supervisor>";

        [SpParam(Ignored = true)]
        public IList<TeamDisciplinePermission> EmployeeDisciplines { get; set; }

        public string SupervisorXml
        {
            get
            {
                var value = new StringBuilder(Constants.BeginRootTag);

                if (EmployeeDisciplines != null)
                {
                    foreach (TeamDisciplinePermission supervisor in EmployeeDisciplines)
                    {
                        value.AppendFormat(
                            SupervisorTpl,
                            supervisor.OwnerId,
                            supervisor.TeamId,
                            supervisor.ModifiedDate.ToString(Constants.ModifiedDateTimeFormat),
                            supervisor.IsDeleted);
                    }
                }

                return value.Append(Constants.EndRootTag).ToString();
            }
        }

        public override string GetSettingKey() => "Sync_TeamDisciplinePermission";

        public override bool IsValid() => EmployeeDisciplines != null && EmployeeDisciplines.Any();
    }
}