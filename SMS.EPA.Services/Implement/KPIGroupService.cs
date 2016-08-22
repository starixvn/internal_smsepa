namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Interfaces;

    public class KPIGroupService : BaseService, IKPIGroupService
    {
        public IEnumerable<KPIGroupModel> GetKPIGroup(GetKPIGroupCriteria criteria)
        {
            IEnumerable<KPIGroupModel> kpiGroups = new List<KPIGroupModel>();

            if (criteria != null && criteria.IsValid())
            {
                kpiGroups = GetData<KPIGroupModel>(criteria);
            }

            return kpiGroups;
        }

        public KPIGroupModel GetKPIGroupById(int kpiGroupId)
        {
            var criteria = new GetKPIGroupByIdCriteria
            {
                KPIGroupId = kpiGroupId
            };
            KPIGroupModel kpiGroup = new KPIGroupModel();

            if (criteria != null && criteria.IsValid())
            {
                kpiGroup = GetDataEntity<KPIGroupModel>(criteria);
            }

            return kpiGroup;
        }

        public CommandResult CreateKPIGroup(CreateKPIGroupCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateKPIGroup(UpdateKPIGroupCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateStatusKPIGroup(UpdateStatusKPIGroupCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult DeleteKPIGroup(DeleteKPIGroupCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Delete(criteria);
            }

            return commandResult;
        }
    }
}