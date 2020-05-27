namespace SMS.EPA.Model.Feedback
{
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    public class FeedbackKPIGroup
    {
        [JsonIgnore]
        public long FeedbackId { get; set; }

        public int KPIGroupId { get; set; }
        public string Name { get; set; }
        public double Weight { get; set; }
        public bool IsSpreadKPI { get; set; }
        public short DisplayOrder { get; set; }
        public bool HasKPIOptional => KPIs.Any(x => !x.IsRequiredEvaluate);

        public IEnumerable<FeedbackKPI> KPIs { get; set; } = new List<FeedbackKPI>();
    }
}