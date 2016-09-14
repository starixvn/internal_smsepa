namespace SMS.EPA.Services.Interfaces
{
    using SMS.EPA.Criteria.TargetType;
    using SMS.EPA.Model.Enitites;
    using System.Collections.Generic;

    public interface ITargetTypeService
    {
        IEnumerable<TargetType> GetTargetType(GetTargetTypeCriteria criteria);
    }
}