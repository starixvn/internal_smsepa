namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.TargetType;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Interfaces;

    public class TargetTypeService : BaseService, ITargetTypeService
    {
        public IEnumerable<TargetTypeModel> GetTargetType(GetTargetTypeCriteria criteria)
        {
            IEnumerable<TargetTypeModel> targetTypes = GetData<TargetTypeModel>(criteria);
            return targetTypes;
        }
    }
}