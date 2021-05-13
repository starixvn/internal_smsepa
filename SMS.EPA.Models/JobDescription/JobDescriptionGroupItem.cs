namespace SMS.EPA.Model.JobDescription
{
    using System.Collections.Generic;
    using System.Linq;

    public class JobDescriptionGroupItem
    {
        public int GroupItemId { get; set; }
        public string Name { get; set; }
        public IEnumerable<JobDescriptionItem> Items { get; set; } = new List<JobDescriptionItem>();
        public bool IsSelectedAll => Items.Any() && Items.All(x => x.IsSelected);
       
    }
}