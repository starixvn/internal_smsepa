namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Services;
    using Criteria;
    using Interfaces;
    using Model;
    using Repositories;

    public class TargetTemplateService : ITargetTemplateService
    {
        private readonly ITargetTemplateRepository targetTemplateRepository;
        private readonly IBaseService baseService;

        public TargetTemplateService() : this(null, null)
        {
        }

        public TargetTemplateService(ITargetTemplateRepository targetTemplateRepository, IBaseService baseService)
        {
            this.targetTemplateRepository = targetTemplateRepository ?? new TargetTemplateRepository();
            this.baseService = baseService ?? new BaseService();
        }

        public IEnumerable<KPIGroupModel> GetKPIGroupOfTargetTemplate(GetKPIGroupOfTargetTemplateCriteria criteria)
        {
            IEnumerable<KPIGroupModel> groups = baseService.GetData<KPIGroupModel>(criteria);
            foreach (KPIGroupModel group in groups.Where(x => x.Weight.HasValue && x.Weight.Value > 0))
            {
                group.Selected = true;
            }

            return groups;
        }

        public IEnumerable<TargetTemplateKPIGroupModel> GetKPIOfTargetTemplate(GetKPIOfTargetTemplateCriteria criteria, bool onlyKPISelected)
        {
            IEnumerable<TargetTemplateKPIGroupModel> kpiGroups = new List<TargetTemplateKPIGroupModel>();

            if (criteria != null && criteria.IsValid())
            {
                IEnumerable<KPIModel> kpiList = targetTemplateRepository.GetKPIOfTargetTemplate(criteria);
                kpiGroups = GroupKPI(kpiList);

                if (onlyKPISelected)
                {
                    kpiGroups = kpiGroups.Where(x => x.KPIs.Any(p => p.Selected == true));
                }
            }

            return kpiGroups;
        }

        private static IEnumerable<TargetTemplateKPIGroupModel> GroupKPI(IEnumerable<KPIModel> kpiList)
        {
            return kpiList.GroupBy(x => x.KPIGroups.First().KPIGroupId).Select(x => new TargetTemplateKPIGroupModel
            {
                KPIGroupId = x.Key,
                Name = x.First().KPIGroups.First().Name,
                IsSpreadKPI = x.First().KPIGroups.First().IsSpreadKPI,
                Weight = x.First().KPIGroups.First().Weight,
                Selected = CheckGroupSelected(x.ToList()),
                KPIs = CheckKPISelected(x.ToList())
            });
        }

        private static bool CheckGroupSelected(List<KPIModel> listKPI)
        {
            bool allSelected = true;
            listKPI.ForEach(x =>
                {
                    if (x.Weight == null)
                    {
                        allSelected = false;
                    }
                });

            return allSelected;
        }

        private static IEnumerable<KPIModel> CheckKPISelected(List<KPIModel> listKPI)
        {
            listKPI.ForEach(x =>
            {
                if (x.Weight >= 0)
                {
                    x.Selected = true;
                }
            });

            return listKPI;
        }
    }
}