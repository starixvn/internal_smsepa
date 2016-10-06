namespace SMS.EPA.ServicesAPI.Criteria.Team
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.ServicesAPI.Model;

    public class UpdateTeamCriteria : CriteriaBase
    {
        private const string _UserTpl = "<Team TeamId='{0}' Code='{1}' "
            + "Name='{2}' LeaderId='{3}' AssistantLeaderId='{4}' Rank='{5}' IsDeleted='{6}' ModifiedDate='{7}' DepartmentId='{8}'></Team>";

        [SpParam(Ignored = true)]
        public IList<Team> Teams { get; set; }

        public string TeamXml
        {
            get
            {
                var value = new StringBuilder("<ROOT>");

                if (Teams != null)
                {
                    foreach (Team team in Teams)
                    {
                        value.AppendFormat(
                            _UserTpl,
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

                return value.Append("</ROOT>").ToString();
            }
        }

        public override string GetSettingKey()
        {
            return "Sync_Group";
        }

        public override bool IsValid()
        {
            return Teams != null && Teams.Any();
        }
    }
}