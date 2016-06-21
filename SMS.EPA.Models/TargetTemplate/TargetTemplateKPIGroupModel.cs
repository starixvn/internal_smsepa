namespace SMS.EPA.Model.TargetTemplate
{
    using System.Collections.Generic;
    using SMS.EPA.Model.KPI;
    using SMS.EPA.Model.KPIGroup;

    public class TargetTemplateKPIGroupModel : KPIGroupModel
    {
        public bool IsSpreadKPIOriginal
        {
            get
            {
                return IsSpreadKPI;
            }
        }

        public IEnumerable<KPIModel> KPIs { get; set; }
    }
}