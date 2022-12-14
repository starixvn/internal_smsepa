namespace SMS.EPA.AutoJob.Models
{
    public class JobSettings
    {
        public string JobName { get; set; }
        public string CronExpression { get; set; }
        public bool IsEnabled { get; set; }
    }
}