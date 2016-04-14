namespace SMS.EPA.ServicesAPI.Criteria.JobTitle
{
    using Fanex.Data.Repository;
    using SMS.EPA.ServicesAPI.Model;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;

    public class UpdateJobTitleCriteria : CriteriaBase
    {
        private const string _JobTitleTpl = "<JobTitle JobTitleId='{0}' Code='{1}' Name='{2}'></JobTitle>";

        [SpParam(Ignored = true)]
        public IList<JobTitle> JobTitles { get; set; }

        public string JobTitleXml
        {
            get
            {
                var value = new StringBuilder("<ROOT>");

                if (JobTitles != null)
                {
                    foreach (JobTitle jobTitle in JobTitles)
                    {
                        value.AppendFormat(
                            _JobTitleTpl,
                            jobTitle.JobTitleId,
                            jobTitle.Code,
                            HttpUtility.HtmlEncode(jobTitle.Name));
                    }
                }

                return value.Append("</ROOT>").ToString();
            }
        }

        public override string GetSettingKey()
        {
            return "Sync_JobTitle";
        }

        public override bool IsValid()
        {
            return JobTitles != null && JobTitles.Any();
        }
    }
}
