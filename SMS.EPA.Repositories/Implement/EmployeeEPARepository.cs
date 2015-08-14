// -----------------------------------------------------------------------
// <copyright file="EmployeeEPARepository.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.04">Created</change>
// </history>

namespace SMS.EPA.Repositories.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.KPI;
    using SMS.EPA.Repositories.Interfaces;

    public class EmployeeEPARepository : IEmployeeEPARepository
    {
        public IEnumerable<EmployeeEPAModel> GetEPAByEmployee(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<EmployeeEPAModel, DepartmentModel, Group, StatusModel, EmployeeModel, EmployeeEPAModel>(
                    (me, dept, group, status, ev) =>
                    {
                        me.Department = dept;
                        me.Group = group;
                        me.Status = status;
                        me.Evaluator = ev;
                        return me;
                    },
                    "DepartmentId, GroupId, StatusId, EvaluatorId",
                    criteria.GetSpParams());
            }
        }

        public EmployeeEPAInfoModel GetEPAInfo(ICriteria criteria)
        {
            EmployeeEPAInfoModel data = new EmployeeEPAInfoModel();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    data = multi.Read<EmployeeEPAInfoModel, StatusModel, EmployeeModel, EmployeeModel, EmployeeEPAInfoModel>(
                        (info, status, evaluator, modifier) =>
                        {
                            info.Status = status;
                            info.Evaluator = evaluator;
                            info.Modifier = modifier;

                            return info;
                        },
                        "StatusId, EvaluatorId, ModifierId").FirstOrDefault();

                    IEnumerable<KPIGroupViewModel> groups = multi.Read<KPIGroupViewModel, KPIModel, KPIGroupViewModel>(
                        (group, kpi) =>
                        {
                            group.KPIs.Add(kpi);

                            return group;
                        },
                        "KPIId");

                    List<KPIGroupViewModel> kpiGroups = GetDistinctKPIGroup(groups);
                    foreach (var kpiGroup in kpiGroups)
                    {
                        GroupKPIByKPIGroup(kpiGroup, groups);
                    }

                    data.KPIGroups = kpiGroups;
                }
            }

            return data;
        }

        private List<KPIGroupViewModel> GetDistinctKPIGroup(IEnumerable<KPIGroupViewModel> groups)
        {
            List<KPIGroupViewModel> result = new List<KPIGroupViewModel>();
            List<int> listId = new List<int>();

            foreach (var group in groups)
            {
                if (!listId.Contains(group.KPIGroupId))
                {
                    listId.Add(group.KPIGroupId);

                    var item = new KPIGroupViewModel()
                    {
                        KPIGroupId = group.KPIGroupId,
                        Name = group.Name,
                        DisplayOrder = group.DisplayOrder,
                        Weight = group.Weight,
                        IsSpreadKPI = group.IsSpreadKPI
                    };

                    result.Add(item);
                }
            }

            return result;
        }

        private void GroupKPIByKPIGroup(KPIGroupViewModel kpiGroup, IEnumerable<KPIGroupViewModel> groups)
        {
            foreach (var group in groups)
            {
                if (group.KPIGroupId == kpiGroup.KPIGroupId)
                {
                    foreach (var kpi in group.KPIs)
                    {
                        kpiGroup.KPIs.Add(kpi);
                    }
                }
            }
        }
    }
}