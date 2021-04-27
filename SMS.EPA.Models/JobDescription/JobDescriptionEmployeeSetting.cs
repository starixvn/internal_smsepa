namespace SMS.EPA.Model.JobDescription
{
    using System.Collections.Generic;

    public class JobDescriptionEmployeeSetting : JobDescriptionEmployeeInfo
    {
        public IEnumerable<JobDescriptionGroupItem> GroupItems { get; set; } = new List<JobDescriptionGroupItem>();
    }
}