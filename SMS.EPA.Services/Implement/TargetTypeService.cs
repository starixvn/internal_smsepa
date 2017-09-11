namespace SMS.EPA.Services.Implement
{
    using Core.Services;
    using Criteria.TargetType;
    using Interfaces;
    using Model;
    using System.Collections.Generic;

    public class TargetTypeService : BaseService, ITargetTypeService
    {
        public IEnumerable<TargetTypeModel> GetTargetType(GetTargetTypeCriteria criteria)
        {
            var targetTypes = GetData<TargetTypeModel>(criteria);
            return targetTypes;
        }
    }
}