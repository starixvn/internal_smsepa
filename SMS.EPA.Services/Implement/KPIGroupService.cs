namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using Core.Services;
    using Criteria;
    using Interfaces;
    using Model;

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
    }
}