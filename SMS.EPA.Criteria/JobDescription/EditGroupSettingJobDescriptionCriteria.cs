namespace SMS.EPA.Criteria.JobDescription
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Model.JobDescription;

    public class EditGroupSettingJobDescriptionCriteria : CriteriaBase
    {
        private const string XmlTemplate = "<Row ItemId='{0}' DisplayOrder='{1}' />";

        public int GroupId { get; set; }
        public int JobLevelId { get; set; }

        [SpParam(Ignored = true)]
        public IEnumerable<JobDescriptionItem> Items { get; set; } = new List<JobDescriptionItem>();

        public string SettingXml
        {
            get
            {
                var xml = new StringBuilder();
                if (Items != null && Items.Any())
                {
                    xml.Append(Constants.BeginRootTag);
                    foreach (var item in Items)
                    {
                        xml.AppendFormat(XmlTemplate, item.ItemId, item.DisplayOrder);
                    }
                    xml.Append(Constants.EndRootTag);
                }

                return xml.ToString();
            }
        }

        public int ModifiedBy { get; set; }

        public override bool IsValid() => GroupId > 0 && JobLevelId > 0;

        public override string GetSettingKey() => "JobDescription_UpdateGroupSetting";
    }
}