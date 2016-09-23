namespace SMS.EPA.Model
{
    using System.Collections.Generic;

    public class TemporaryKPIGroupModel
    {
        public TemporaryKPIGroupModel()
        {
            KPIs = new List<TemporaryKPIModel>();
            KPIsBySelf = new List<TemporaryKPIModel>();
        }

        public int KPIGroupId { get; set; }

        public string Name { get; set; }

        public double? Weight { get; set; }

        public bool IsSpreadKPI { get; set; }

        public int NumberKPI { get; set; }

        public bool HasKPIOptional { get; set; }

        public int NumberKPIOptional { get; set; }

        public IList<TemporaryKPIModel> KPIs { get; set; }

        public IList<TemporaryKPIModel> KPIsBySelf { get; set; }
    }
}