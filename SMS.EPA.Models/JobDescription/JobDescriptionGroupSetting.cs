namespace SMS.EPA.Model.JobDescription
{
    using System.Collections.Generic;

    public class JobDescriptionGroupSetting : JobDescriptionGroupInfo
    {
        public IEnumerable<JobDescriptionGroupItem> GroupItems { get; set; } = new List<JobDescriptionGroupItem>();
    }
}