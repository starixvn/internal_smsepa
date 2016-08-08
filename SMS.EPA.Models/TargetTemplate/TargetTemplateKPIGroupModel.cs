namespace SMS.EPA.Model.TargetTemplate
{
    using System.Collections.Generic;

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