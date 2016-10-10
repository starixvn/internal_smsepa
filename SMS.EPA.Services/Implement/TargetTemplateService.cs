namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Repositories;

    using SMS.EPA.Services.Interfaces;

    public class TargetTemplateService : BaseService, ITargetTemplateService
    {
        private readonly ITargetTemplateRepository targetTemplateRepository;

        public TargetTemplateService()
            : this(null)
        {
        }

        public TargetTemplateService(ITargetTemplateRepository targetTemplateRepository)
        {
            this.targetTemplateRepository = targetTemplateRepository ?? new TargetTemplateRepository();
        }

        public TargetTemplate GetTargetTemplateById(GetTargetTemplateByIdCriteria criteria)
        {
            TargetTemplate targetTemplate = new TargetTemplate();

            if (criteria != null && criteria.IsValid())
            {
                targetTemplate = GetDataEntity<TargetTemplate>(criteria);
            }

            return targetTemplate;
        }

        public IEnumerable<TargetTemplate> GetAllTargetTemplate(GetAllTargetTemplateCriteria criteria)
        {
            IEnumerable<TargetTemplate> targetTemplates = new List<TargetTemplate>();

            if (criteria != null && criteria.IsValid())
            {
                targetTemplates = GetData<TargetTemplate>(criteria);
            }

            return targetTemplates;
        }

        public CommandResult CreateTargetTemplate(CreateTargetTemplateCriteria criteria)
        {
            CommandResult result = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                result = Repository.Insert(criteria);
            }

            return result;
        }

        public CommandResult UpdateTargetTemplate(UpdateTargetTemplateCriteria criteria)
        {
            CommandResult result = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                result = Repository.Update(criteria);
            }

            return result;
        }

        public CommandResult UpdateTargetTemplateBasicInfo(UpdateTargetTemplateBasicInfoCriteria criteria)
        {
            CommandResult result = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                result = Repository.Update(criteria);
            }

            return result;
        }

        public CommandResult DeleteTargetTemplate(DeleteTargetTemplateCriteria criteria)
        {
            CommandResult result = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                result = Repository.Delete(criteria);
            }

            return result;
        }

        public CommandResult AddKPIGroupWeight(SetKPIGroupOfTargetTemplateCriteria criteria)
        {
            CommandResult result = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                result = Repository.Insert(criteria);
            }

            return result;
        }

        public IEnumerable<KPIGroupModel> GetKPIGroupOfTargetTemplate(GetKPIGroupOfTargetTemplateCriteria criteria)
        {
            IEnumerable<KPIGroupModel> groups = new List<KPIGroupModel>();

            if (criteria != null && criteria.IsValid())
            {
                groups = GetData<KPIGroupModel>(criteria);

                foreach (KPIGroupModel group in groups.Where(x => x.Weight.HasValue && x.Weight.Value > 0))
                {
                    group.Selected = true;
                }
            }

            return groups;
        }

        public CommandResult UpdateStatusTargetTemplate(UpdateStatusTargetTemplateCriteria criteria)
        {
            CommandResult result = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                result = Repository.Update(criteria);
            }

            return result;
        }

        public CommandResult DeleteKPIGroup(DeleteKPIGroupOfTargetTemplateCriteria criteria)
        {
            CommandResult result = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                result = Repository.Delete(criteria);
            }

            return result;
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

        public IEnumerable<RatingType> GetRatingType(GetRatingTypeCriteria criteria)
        {
            IEnumerable<RatingType> ratingTypes = new List<RatingType>();

            if (criteria != null && criteria.IsValid())
            {
                ratingTypes = GetData<RatingType>(criteria);
            }

            return ratingTypes;
        }

        private IEnumerable<TargetTemplateKPIGroupModel> GroupKPI(IEnumerable<KPIModel> kpiList)
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

        private bool CheckGroupSelected(List<KPIModel> listKPI)
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

        private IEnumerable<KPIModel> CheckKPISelected(List<KPIModel> listKPI)
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