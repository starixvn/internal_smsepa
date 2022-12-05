namespace SMS.EPA.AutoJob.Models
{
    public class JobSetting
    {
        public string JobName { get; set; }

        public bool IsEnabled { get; set; }

        public int TimeInterval { get; set; }
    }
}
