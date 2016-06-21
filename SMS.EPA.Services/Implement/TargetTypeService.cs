namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.TargetType;
    using SMS.EPA.Model.TargetType;
    using SMS.EPA.Services.Interfaces;

    public class TargetTypeService : BaseService, ITargetTypeService
    {
        public IEnumerable<TargetTypeModel> GetTargetType(GetTargetTypeCriteria criteria)
        {
            IEnumerable<TargetTypeModel> kpis = new List<TargetTypeModel>();

            if (criteria != null && criteria.IsValid())
            {
                kpis = GetData<TargetTypeModel>(criteria);
            }

            return kpis;
        }
    }
}