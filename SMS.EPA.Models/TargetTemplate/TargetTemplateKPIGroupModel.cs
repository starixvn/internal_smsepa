namespace SMS.EPA.Model
{
    using System.Collections.Generic;

    public class TargetTemplateKPIGroupModel : KPIGroupModel
    {
        public bool IsSpreadKPIOriginal => IsSpreadKPI;

        public IEnumerable<KPIModel> KPIs { get; set; }
    }
}