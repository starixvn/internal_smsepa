namespace SMS.EPA.ServicesAPI.Criteria.JobTitle
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;
    using Core.Constants;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model.API;

    public class UpdateJobTitleCriteria : CriteriaBase
    {
        private const string JobTitleTpl = "<JobTitle JobTitleId='{0}' Code='{1}' Name='{2}'></JobTitle>";

        [SpParam(Ignored = true)]
        public IList<JobTitle> JobTitles { get; set; }

        public string JobTitleXml
        {
            get
            {
                var value = new StringBuilder(Constants.BeginRootTag);

                if (JobTitles != null)
                {
                    foreach (JobTitle jobTitle in JobTitles)
                    {
                        value.AppendFormat(
                            JobTitleTpl,
                            jobTitle.JobTitleId,
                            jobTitle.Code,
                            HttpUtility.HtmlEncode(jobTitle.Name));
                    }
                }

                return value.Append(Constants.EndRootTag).ToString();
            }
        }

        public override string GetSettingKey() => "Sync_JobTitle";

        public override bool IsValid() => JobTitles != null && JobTitles.Any();
    }
}