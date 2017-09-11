namespace SMS.EPA.Services.Implement
{
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Interfaces;
    using Model;
    using System.Collections.Generic;

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
            var kpiGroup = new KPIGroupModel();

            if (criteria.IsValid())
            {
                kpiGroup = GetDataEntity<KPIGroupModel>(criteria);
            }

            return kpiGroup;
        }

        public CommandResult CreateKPIGroup(CreateKPIGroupCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateKPIGroup(UpdateKPIGroupCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateStatusKPIGroup(UpdateStatusKPIGroupCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult DeleteKPIGroup(DeleteKPIGroupCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Delete(criteria);
            }

            return commandResult;
        }
    }
}