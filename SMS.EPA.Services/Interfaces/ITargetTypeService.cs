namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria.TargetType;
    using Model;

    public interface ITargetTypeService
    {
        IEnumerable<TargetTypeModel> GetTargetType(GetTargetTypeCriteria criteria);
    }
}