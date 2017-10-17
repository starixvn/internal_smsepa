namespace SMS.EPA.ServicesAPI.Criteria.Team
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;
    using Core.Constants;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model.API;

    public class UpdateTeamCriteria : CriteriaBase
    {
        private const string UserTpl = "<Team TeamId='{0}' Code='{1}' "
            + " Name='{2}' LeaderId='{3}' AssistantLeaderId='{4}' Rank='{5}' IsDeleted='{6}' ModifiedDate='{7}' DepartmentId='{8}'></Team>";

        [SpParam(Ignored = true)]
        public IList<Team> Teams { get; set; }

        public string TeamXml
        {
            get
            {
                var value = new StringBuilder(Constants.BeginRootTag);

                if (Teams != null)
                {
                    foreach (Team team in Teams)
                    {
                        value.AppendFormat(
                            UserTpl,
                            team.TeamId,
                            HttpUtility.HtmlEncode(team.Code),
                            HttpUtility.HtmlEncode(team.Name),
                            team.LeaderId,
                            team.AssistantLeaderId,
                            team.Rank,
                            team.IsDeleted,
                            team.ModifiedDate,
                            team.DepartmentId);
                    }
                }

                return value.Append(Constants.EndRootTag).ToString();
            }
        }

        public override string GetSettingKey() => "Sync_Group";

        public override bool IsValid() => Teams != null && Teams.Any();
    }
}