namespace SMS.EPA.Model.Feedback
{
    using Newtonsoft.Json;

    public class FeedbackKPI
    {
        [JsonIgnore]
        public long FeedbackId { get; set; }

        [JsonIgnore]
        public int KPIGroupId { get; set; }

        public int KPIId { get; set; }
        public string Name { get; set; }
        public bool HasComment { get; set; }
        public bool IsRequiredComment { get; set; }
        public bool IsRequiredEvaluate { get; set; }
        public double Weight { get; set; }
        public string Score { get; set; }
        public string Rate { get; set; }
        public string Comment { get; set; }
        public short DisplayOrder { get; set; }
    }
}