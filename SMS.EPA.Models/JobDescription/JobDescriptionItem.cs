namespace SMS.EPA.Model.JobDescription
{
    public class JobDescriptionItem
    {
        public int GroupItemId { get; set; }
        public int ItemId { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
        public short DisplayOrder { get; set; }
    }
}