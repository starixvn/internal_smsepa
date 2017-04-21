namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using Core.Model;
    using Criteria;
    using Model;

    public interface IKPIGroupService
    {
        IEnumerable<KPIGroupModel> GetKPIGroup(GetKPIGroupCriteria criteria);

        KPIGroupModel GetKPIGroupById(int kpiGroupId);

        CommandResult CreateKPIGroup(CreateKPIGroupCriteria criteria);

        CommandResult UpdateKPIGroup(UpdateKPIGroupCriteria criteria);

        CommandResult UpdateStatusKPIGroup(UpdateStatusKPIGroupCriteria criteria);

        CommandResult DeleteKPIGroup(DeleteKPIGroupCriteria criteria);
    }
}