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
    using SMS.EPA.Model.TargetType;
    using SMS.EPA.Repositories.Interfaces;

    public class EmployeeEPARepository : IEmployeeEPARepository
    {
        public IEnumerable<EmployeeEPAModel> GetEPAByEmployee(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IEnumerable<EmployeeEPAModel> epas = multi.Read<EmployeeEPAModel, Employee, TargetTypeModel, DepartmentModel, Group, StatusModel, EmployeeEPAModel>(
                    (me, emp, target, dept, group, status) =>
                    {
                        me.Employee = emp;
                        me.TargetType = target;
                        me.Department = dept;
                        me.Group = group;
                        me.Status = status;

                        return me;
                    },
                    "EmployeeId, TargetTypeId, DepartmentId, GroupId, StatusId");

                    IEnumerable<RawEmployeeEPAModel> evaluators = multi.Read<RawEmployeeEPAModel, EmployeeModel, RawEmployeeEPAModel>(
                        (result, eva) =>
                        {
                            result.Evaluator = eva;

                            return result;
                        },
                        "EvaluatorId");

                    epas = GroupEvaluator(epas, evaluators);

                    return epas;
                }
            }
        }

        public EmployeeEPAInfoModel GetEPAInfo(ICriteria criteria)
        {
            EmployeeEPAInfoModel data = new EmployeeEPAInfoModel();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    data = multi.Read<EmployeeEPAInfoModel, StatusModel, Employee, EmployeeEPAInfoModel>(
                        (info, status, modifier) =>
                        {
                            info.Status = status;
                            info.Modifier = modifier;

                            return info;
                        },
                        "StatusId, ModifierId").FirstOrDefault();

                    data.Evaluators = multi.Read<Employee>().ToList();

                    IEnumerable<KPIGroupViewModel> groups = multi.Read<KPIGroupViewModel, KPIViewModel, KPIGroupViewModel>(
                        (group, kpi) =>
                        {
                            group.KPIs.Add(kpi);

                            return group;
                        },
                        "KPIId");

                    IList<KPIGroupViewModel> kpiGroups = GetDistinctKPIGroup(groups);

                    foreach (var kpiGroup in kpiGroups)
                    {
                        GroupKPIByKPIGroup(kpiGroup, groups);
                    }

                    data.KPIGroups = kpiGroups;
                }
            }

            return data;
        }

        public IEnumerable<MemberEPAModel> GetEPAByEvaluator(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<MemberEPAModel, Employee, Group, TargetTypeModel, StatusModel, MemberEPAModel>(
                    (epa, emp, gr, tt, status) =>
                    {
                        epa.Employee = emp;
                        epa.Group = gr;
                        epa.TargetType = tt;
                        epa.Status = status;

                        return epa;
                    },
                    "EmployeeId, GroupId, TargetTypeId, StatusId",
                    criteria.GetSpParams());
            }
        }

        public EmployeeEPAInfoModel GetEPAInfoByEvaluator(ICriteria criteria)
        {
            EmployeeEPAInfoModel data = new EmployeeEPAInfoModel();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    data = multi.Read<EmployeeEPAInfoModel, StatusModel, Employee, EmployeeEPAInfoModel>(
                        (info, status, modifier) =>
                        {
                            info.Status = status;
                            info.Modifier = modifier;

                            return info;
                        },
                        "StatusId, ModifierId").FirstOrDefault();

                    data.Evaluators = multi.Read<Employee>().ToList();

                    IEnumerable<KPIGroupViewModel> groups = multi.Read<KPIGroupViewModel, KPIViewModel, KPIGroupViewModel>(
                        (group, kpi) =>
                        {
                            group.KPIs.Add(kpi);

                            return group;
                        },
                        "KPIId");

                    IList<KPIGroupViewModel> kpiGroups = GetDistinctKPIGroup(groups);

                    foreach (var kpiGroup in kpiGroups)
                    {
                        GroupKPIByKPIGroup(kpiGroup, groups);
                    }

                    data.KPIGroups = kpiGroups;
                }
            }

            return data;
        }

        public IEnumerable<MemberEPAModel> GetEPAByReviewer(ICriteria criteria)
        {
            IEnumerable<MemberEPAModel> memberEPAs = new List<MemberEPAModel>();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    memberEPAs = multi.Read<MemberEPAModel, Employee, Group, TargetTypeModel, StatusModel, MemberEPAModel>(
                    (epa, emp, gr, tt, status) =>
                    {
                        epa.Employee = emp;
                        epa.Group = gr;
                        epa.TargetType = tt;
                        epa.Status = status;

                        return epa;
                    },
                    "EmployeeId, GroupId, TargetTypeId, StatusId");

                    IEnumerable<EvaluatorModel> evaluators = multi.Read<EvaluatorModel>();

                    memberEPAs = GroupEvaluator(memberEPAs, evaluators);
                }
            }

            return memberEPAs;
        }

        private IEnumerable<EmployeeEPAModel> GroupEvaluator(IEnumerable<EmployeeEPAModel> epas, IEnumerable<RawEmployeeEPAModel> evaluators)
        {
            foreach (var epa in epas)
            {
                foreach (var evaluator in evaluators)
                {
                    if (evaluator.EPAId == epa.EPAId)
                    {
                        epa.Evaluators.Add(evaluator.Evaluator);
                    }
                }
            }

            return epas;
        }

        private IEnumerable<MemberEPAModel> GroupEvaluator(IEnumerable<MemberEPAModel> memberEPAs, IEnumerable<EvaluatorModel> evaluators)
        {
            foreach (var memberEPA in memberEPAs)
            {
                foreach (var evaluator in evaluators)
                {
                    if (evaluator.EPAId == memberEPA.EPAId)
                    {
                        memberEPA.Evaluators.Add(evaluator);
                    }
                }
            }

            return memberEPAs;
        }

        private IList<KPIGroupViewModel> GetDistinctKPIGroup(IEnumerable<KPIGroupViewModel> groups)
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