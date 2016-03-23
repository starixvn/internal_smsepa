// ----------------------------------------------------------------------
// <copyright file="TargetTemplateService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/10/2015 12:34:40 PM">Created.</change>
// </history>

namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.TargetTemplate;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.KPI;
    using SMS.EPA.Model.KPIGroup;
    using SMS.EPA.Model.TargetTemplate;
    using SMS.EPA.Repositories.Implement;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Services.Interfaces;

    public class TargetTemplateService : BaseService, ITargetTemplateService
    {
        private readonly ITargetTemplateRepository _targetTemplateRepository;

        public TargetTemplateService()
            : this(null)
        {
        }

        public TargetTemplateService(ITargetTemplateRepository targetTemplateRepository)
        {
            _targetTemplateRepository = targetTemplateRepository ?? new TargetTemplateRepository();
        }

        public TargetTemplateModel GetTargetTemplateById(GetTargetTemplateByIdCriteria criteria)
        {
            TargetTemplateModel targetTemplate = new TargetTemplateModel();

            if (criteria != null && criteria.IsValid())
            {
                targetTemplate = GetDataEntity<TargetTemplateModel>(criteria);
            }

            return targetTemplate;
        }

        public IEnumerable<TargetTemplateModel> GetAllTargetTemplate(GetAllTargetTemplateCriteria criteria)
        {
            IEnumerable<TargetTemplateModel> targetTemplates = new List<TargetTemplateModel>();

            if (criteria != null && criteria.IsValid())
            {
                targetTemplates = GetData<TargetTemplateModel>(criteria);
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

        public CommandResult UpdateTargetTemplateBasicInformation(UpdateTargetTemplateBasicInformationCriteria criteria)
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

                foreach (KPIGroupModel group in groups)
                {
                    if (group.Weight > 0)
                    {
                        group.Selected = true;
                    }
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
                IEnumerable<KPIModel> kpiList = _targetTemplateRepository.GetKPIOfTargetTemplate(criteria);
                kpiGroups = GroupKPI(kpiList);

                if (onlyKPISelected)
                {
                    kpiGroups = GetSelectedKPI(kpiGroups);
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

        private IEnumerable<TargetTemplateKPIGroupModel> GetSelectedKPI(IEnumerable<TargetTemplateKPIGroupModel> kpiGroups)
        {
            return kpiGroups.Where(x => CheckHasSelectedKPI(x));
        }

        private bool CheckHasSelectedKPI(TargetTemplateKPIGroupModel group)
        {
            bool hasSelectedKPI = false;

            foreach (KPIModel item in group.KPIs)
            {
                if (item.Selected == true)
                {
                    hasSelectedKPI = true;
                }
            }

            return hasSelectedKPI;
        }
    }
}