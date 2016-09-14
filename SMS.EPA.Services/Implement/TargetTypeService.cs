namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.TargetType;
    using SMS.EPA.Services.Interfaces;
    using SMS.EPA.Model.Enitites;

    public class TargetTypeService : BaseService, ITargetTypeService
    {
        public IEnumerable<TargetType> GetTargetType(GetTargetTypeCriteria criteria)
        {
            IEnumerable<TargetType> targetTypes = GetData<TargetType>(criteria);
            return targetTypes;
        }
    }
}