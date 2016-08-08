namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.KPIGroup;
    using SMS.EPA.Model;

    public interface IKPIGroupService
    {
        IEnumerable<KPIGroupModel> GetKPIGroup(GetKPIGroupCriteria criteria);

        KPIGroupModel GetKPIGroupById(GetKPIGroupByIdCriteria criteria);

        CommandResult CreateKPIGroup(CreateKPIGroupCriteria criteria);

        CommandResult UpdateKPIGroup(UpdateKPIGroupCriteria criteria);

        CommandResult UpdateStatusKPIGroup(UpdateStatusKPIGroupCriteria criteria);

        CommandResult DeleteKPIGroup(DeleteKPIGroupCriteria criteria);
    }
}