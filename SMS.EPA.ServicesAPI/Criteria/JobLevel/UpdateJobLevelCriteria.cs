namespace SMS.EPA.ServicesAPI.Criteria.JobLevel
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;
    using Core.Constants;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model.API;

    public class UpdateJobLevelCriteria : CriteriaBase
    {
        private const string JobLevelTpl = "<JobLevel JobLevelId='{0}' Name='{1}' IsActive='{2}' ModifiedDate='{3}'></JobLevel>";

        [SpParam(Ignored = true)]
        public IList<JobLevel> JobLevels { get; set; }

        public string JobLevelXml
        {
            get
            {
                var value = new StringBuilder(Constants.BeginRootTag);

                foreach (JobLevel jobLevel in JobLevels)
                {
                    value.AppendFormat(
                        JobLevelTpl,
                        jobLevel.JobLevelId,
                        HttpUtility.HtmlEncode(jobLevel.Name),
                        jobLevel.IsActive,
                        jobLevel.ModifiedDate);
                }

                return value.Append(Constants.EndRootTag).ToString();
            }
        }

        public override string GetSettingKey() => "Sync_JobLevel";

        public override bool IsValid() => JobLevels != null && JobLevels.Any();
    }
}