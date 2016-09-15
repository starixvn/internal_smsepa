namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.TargetType;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Services.Interfaces;

    public class TargetTypeService : BaseService, ITargetTypeService
    {
        public IEnumerable<TargetType> GetTargetType(GetTargetTypeCriteria criteria)
        {
            IEnumerable<TargetType> targetTypes = GetData<TargetType>(criteria);
            return targetTypes;
        }
    }
}