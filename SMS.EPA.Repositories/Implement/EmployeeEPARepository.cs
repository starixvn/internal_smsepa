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
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.EmployeeEPA.RatingByKPI;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.Report.Dashboard;
    using SMS.EPA.Model.ScoreRatingType;
    using SMS.EPA.Repositories.Interfaces;

    public class EmployeeEPARepository : IEmployeeEPARepository
    {
        public IEnumerable<EPAModel> GetEPA(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IEnumerable<EPAModel> epaInfos = multi.Read<EPAModel, EPAEmployeeModel, RatingTypeModel, StatusModel, EPAPeriodModel, ScoreRatingTypeModel, EPAModel>(
                    (epaInfo, employee, ratingType, status, period, scoreRatingType) =>
                    {
                        epaInfo.Employee = employee;
                        epaInfo.RatingType = ratingType;
                        epaInfo.ScoreRatingType = scoreRatingType;
                        epaInfo.Status = status;
                        epaInfo.Period = period;

                        return epaInfo;
                    },
                    "EmployeeId, RatingTypeId, StatusId, PeriodId, ScoreRatingTypeId");

                    IEnumerable<EPAProcessModel> processes = multi.Read<EPAProcessModel>();

                    GroupEPAProcessByEPA(epaInfos, processes);

                    foreach (EPAModel epaInfo in epaInfos)
                    {
                        DecodeAllScore(epaInfo);
                    }

                    return epaInfos;
                }
            }
        }

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Need to review here.")]
        public EPAModel GetEPAInfo(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    EPAModel epaInfo = new EPAModel();

                    epaInfo = multi.Read<EPAModel, EPAEmployeeModel, RatingTypeModel, StatusModel, EPAPeriodModel, ScoreRatingTypeModel, EPAModel>(
                    (info, employee, ratingType, status, period, scoreRatingType) =>
                    {
                        info.Employee = employee;
                        info.RatingType = ratingType;
                        info.ScoreRatingType = scoreRatingType;
                        info.Status = status;
                        info.Period = period;

                        return info;
                    },
                    "EmployeeId, RatingTypeId, StatusId, PeriodId, ScoreRatingTypeId").FirstOrDefault();

                    if (epaInfo != null)
                    {
                        IEnumerable<EPAProcessModel> processes = multi.Read<EPAProcessModel>();

                        epaInfo.KPIGroups = multi.Read<EPAKPIGroupModel>().ToList();

                        IEnumerable<EPAKPIModel> kpis = multi.Read<EPAKPIModel>();

                        IEnumerable<EvaluationModel> evaluations = multi.Read<EvaluationModel>();

                        epaInfo.LastProcesses = multi.Read<EPAProcessModel>().ToList();

                        GroupEPAProcessByEPA(epaInfo, processes);
                        GroupKPIByKPIGroup(epaInfo.KPIGroups, kpis);

                        foreach (EPAKPIGroupModel kpiGroup in epaInfo.KPIGroups)
                        {
                            GroupEvaluationOnEachKPI(kpiGroup.KPIs, evaluations);
                        }

                        DecodeAllScore(epaInfo);
                    }

                    return epaInfo;
                }
            }
        }

        public IEnumerable<EPAModel> GetEPAEndEvaluationPeriod(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IEnumerable<EPAModel> epaInfos = multi.Read<EPAModel, StatusModel, EPAEmployeeModel, EPAPeriodModel, EPAModel>(
                    (epaInfo, status, emp, period) =>
                    {
                        epaInfo.Status = status;
                        epaInfo.Employee = emp;
                        epaInfo.Period = period;

                        return epaInfo;
                    },
                    "StatusId, EmployeeId, PeriodId");

                    IEnumerable<EPAProcessModel> processes = multi.Read<EPAProcessModel>();

                    GroupEPAProcessByEPA(epaInfos, processes);

                    foreach (EPAModel epaInfo in epaInfos)
                    {
                        DecodeAllScore(epaInfo);
                    }

                    return epaInfos;
                }
            }
        }

        public DashboardModel GetDashboard(ICriteria criteria)
        {
            DashboardModel result = new DashboardModel();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    result.MyEPAs = multi.Read<EPAModel, StatusModel, EPAPeriodModel, EPAModel>(
                        (epa, status, period) =>
                        {
                            epa.Status = status;
                            epa.Period = period;

                            return epa;
                        },
                        "StatusId, PeriodId");

                    IEnumerable<EPAProcessModel> processes = multi.Read<EPAProcessModel>();

                    GroupEPAProcessByEPA(result.MyEPAs, processes);

                    foreach (EPAModel epaInfo in result.MyEPAs)
                    {
                        DecodeAllScore(epaInfo);
                    }
                }
            }

            return result;
        }

        #region Rating By KPI

        public IEnumerable<TemporaryKPIModel> GetKPIByTargetTemplate(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<TemporaryKPIModel, TemporaryKPIGroupModel, TemporaryKPIModel>(
                    (kpi, gr) =>
                    {
                        kpi.KPIGroup = gr;

                        return kpi;
                    },
                    "KPIGroupId",
                    criteria.GetSpParams());
            }
        }

        public IEnumerable<TemporaryKPIModel> GetKPIToView(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<TemporaryKPIModel, TemporaryKPIGroupModel, TemporaryKPIModel>(
                    (kpi, gr) =>
                    {
                        kpi.KPIGroup = gr;

                        return kpi;
                    },
                    "KPIGroupId",
                    criteria.GetSpParams());
            }
        }

        public IEnumerable<RatingByKPIModel> GetEmployeeEPAByKPI(ICriteria criteria)
        {
            IEnumerable<RatingByKPIModel> epaInfos = new List<RatingByKPIModel>();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    epaInfos = multi.Read<RatingByKPIModel, EPAEmployeeModel, RatingTypeModel, TemporaryKPIGroupModel, RatingByKPIModel>(
                        (epaInfo, employee, type, kgr) =>
                        {
                            epaInfo.Employee = employee;
                            epaInfo.RatingType = type;
                            epaInfo.KPIGroup = kgr;
                            return epaInfo;
                        },
                        "EmployeeId, RatingTypeId, KPIGroupId");

                    IEnumerable<TemporaryKPIModel> kpis = multi.Read<TemporaryKPIModel, TemporaryKPIGroupModel, TemporaryKPIModel>(
                        (kpi, kgr) =>
                        {
                            kpi.KPIGroup = kgr;
                            return kpi;
                        },
                        "KPIGroupId");

                    IEnumerable<TemporaryKPIModel> kpisBySelf = multi.Read<TemporaryKPIModel, TemporaryKPIGroupModel, TemporaryKPIModel>(
                       (kpi, kgr) =>
                       {
                           kpi.KPIGroup = kgr;
                           return kpi;
                       },
                       "KPIGroupId");
                    GroupKPIByEPA(epaInfos, kpis);
                    GroupKPIBySelf(epaInfos, kpisBySelf);
                    DecodeAllScore(epaInfos);
                }
            }

            return epaInfos;
        }

        public IEnumerable<RatingByKPIModel> GetEmployeeEPAToView(ICriteria criteria)
        {
            IEnumerable<RatingByKPIModel> epaInfos = new List<RatingByKPIModel>();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    epaInfos = multi.Read<RatingByKPIModel, EPAEmployeeModel, RatingTypeModel, TemporaryKPIGroupModel, RatingByKPIModel>(
                        (epaInfo, employee, type, kgr) =>
                        {
                            epaInfo.Employee = employee;
                            epaInfo.RatingType = type;
                            epaInfo.KPIGroup = kgr;
                            return epaInfo;
                        },
                        "EmployeeId, RatingTypeId, KPIGroupId");

                    IEnumerable<TemporaryKPIModel> kpis = multi.Read<TemporaryKPIModel, TemporaryKPIGroupModel, TemporaryKPIModel>(
                        (kpi, kgr) =>
                        {
                            kpi.KPIGroup = kgr;
                            return kpi;
                        },
                        "KPIGroupId");

                    IEnumerable<TemporaryKPIModel> kpisBySelf = multi.Read<TemporaryKPIModel, TemporaryKPIGroupModel, TemporaryKPIModel>(
                       (kpi, kgr) =>
                       {
                           kpi.KPIGroup = kgr;
                           return kpi;
                       },
                       "KPIGroupId");
                    GroupKPIByEPA(epaInfos, kpis);
                    GroupKPIBySelf(epaInfos, kpisBySelf);
                    DecodeAllScore(epaInfos);
                }
            }

            return epaInfos;
        }

        #endregion Rating By KPI

        private void GroupEPAProcessByEPA(EPAModel epaInfo, IEnumerable<EPAProcessModel> processes)
        {
            foreach (EPAProcessModel process in processes.Where(x => x.EPAId.Equals(epaInfo.EPAId)))
            {
                switch (process.ProcessId)
                {
                    case (int)ProcessType.HumanResource:
                        epaInfo.Process.HumanResources.Add(process);
                        break;

                    case (int)ProcessType.Self:
                        epaInfo.Process.Self = process;
                        break;

                    case (int)ProcessType.Evaluator:
                        epaInfo.Process.Evaluators.Add(process);
                        break;

                    case (int)ProcessType.Reviewer:
                        epaInfo.Process.Reviewers.Add(process);
                        break;

                    case (int)ProcessType.Management:
                        epaInfo.Process.Managements.Add(process);
                        break;
                }
            }
        }

        private void GroupEPAProcessByEPA(IEnumerable<EPAModel> epaInfos, IEnumerable<EPAProcessModel> processes)
        {
            foreach (EPAModel epaInfo in epaInfos)
            {
                GroupEPAProcessByEPA(epaInfo, processes);
            }
        }

        private void GroupKPIByKPIGroup(IList<EPAKPIGroupModel> kpiGroups, IEnumerable<EPAKPIModel> kpis)
        {
            foreach (EPAKPIGroupModel kpiGroup in kpiGroups)
            {
                foreach (EPAKPIModel kpi in kpis.Where(x => x.EPAId.Equals(kpiGroup.EPAId) && x.KPIGroupId.Equals(kpiGroup.KPIGroupId)))
                {
                    kpiGroup.KPIs.Add(kpi);
                }
            }
        }

        private void GroupEvaluationOnEachKPI(IEnumerable<EPAKPIModel> kpis, IEnumerable<EvaluationModel> evaluations)
        {
            foreach (EPAKPIModel kpi in kpis)
            {
                foreach (EvaluationModel evaluation in evaluations.Where(x => x.EPAId.Equals(kpi.EPAId) && x.KPIId.Equals(kpi.KPIId)))
                {
                    switch (evaluation.ProcessId)
                    {
                        case (int)ProcessType.Self:
                            kpi.EvaluationBySelf = evaluation;
                            break;

                        case (int)ProcessType.Evaluator:
                            kpi.EvaluationByEvaluators.Add(evaluation);
                            break;
                    }
                }
            }
        }

        private void GroupKPIByEPA(IEnumerable<RatingByKPIModel> epaInfos, IEnumerable<TemporaryKPIModel> kpis)
        {
            foreach (RatingByKPIModel epaInfo in epaInfos)
            {
                foreach (TemporaryKPIModel kpi in kpis.Where(x => x.EPAId.Equals(epaInfo.EPAId) && x.KPIGroup.KPIGroupId.Equals(epaInfo.KPIGroup.KPIGroupId)))
                {
                    epaInfo.KPIGroup.KPIs.Add(kpi);
                }
            }
        }

        private void GroupKPIBySelf(IEnumerable<RatingByKPIModel> epaInfos, IEnumerable<TemporaryKPIModel> kpis)
        {
            foreach (RatingByKPIModel epaInfo in epaInfos)
            {
                foreach (TemporaryKPIModel kpi in kpis.Where(x => x.EPAId.Equals(epaInfo.EPAId) && x.KPIGroup.KPIGroupId.Equals(epaInfo.KPIGroup.KPIGroupId)))
                {
                    epaInfo.KPIGroup.KPIsBySelf.Add(kpi);
                }
            }
        }

        private void DecodeAllScore(EPAModel epaInfo)
        {
            epaInfo.LastFinalScore = EncodeService.Decode(epaInfo.LastFinalScoreEncode);
            epaInfo.LastGrade = EncodeService.Decode(epaInfo.LastGradeEncode);
            epaInfo.FinalScore = EncodeService.Decode(epaInfo.FinalScoreEncode);
            epaInfo.Grade = EncodeService.Decode(epaInfo.GradeEncode);
            DecodeTotalScore(epaInfo.Process.Self);
            DecodeTotalScore(epaInfo.Process.Evaluators);
            DecodeTotalScore(epaInfo.Process.Reviewers);
            DecodeTotalScore(epaInfo.Process.Managements);
            DecodeTotalScore(epaInfo.LastProcesses);

            foreach (EPAKPIModel kpi in epaInfo.KPIGroups.SelectMany(x => x.KPIs))
            {
                DecodeScore(kpi.EvaluationBySelf);
                DecodeScore(kpi.EvaluationByEvaluators);
            }
        }

        private void DecodeAllScore(IEnumerable<RatingByKPIModel> epaInfos)
        {
            foreach (RatingByKPIModel epaInfo in epaInfos)
            {
                epaInfo.TotalScore = EncodeService.Decode(epaInfo.TotalScoreEncode);

                foreach (TemporaryKPIModel kpi in epaInfo.KPIGroup.KPIs)
                {
                    float rate = 0;

                    if (!string.IsNullOrEmpty(kpi.RateEncode))
                    {
                        rate = float.Parse(EncodeService.Decode(kpi.RateEncode));
                    }

                    kpi.Score = EncodeService.Decode(kpi.ScoreEncode).Replace("_" + kpi.KPIId, string.Empty);
                    kpi.Rate = rate;
                }

                foreach (TemporaryKPIModel kpi in epaInfo.KPIGroup.KPIsBySelf)
                {
                    float rate = 0;

                    if (!string.IsNullOrEmpty(kpi.RateEncode))
                    {
                        rate = float.Parse(EncodeService.Decode(kpi.RateEncode));
                    }

                    kpi.Score = EncodeService.Decode(kpi.ScoreEncode).Replace("_" + kpi.KPIId, string.Empty);
                    kpi.Rate = rate;
                }
            }
        }

        private void DecodeTotalScore(EPAProcessModel process)
        {
            if (process != null)
            {
                process.TotalScore = EncodeService.Decode(process.TotalScoreEncode);
                process.AdjustScore = EncodeService.Decode(process.AdjustScoreEncode);
            }
        }

        private void DecodeTotalScore(IList<EPAProcessModel> processes)
        {
            foreach (EPAProcessModel process in processes)
            {
                DecodeTotalScore(process);
            }
        }

        private void DecodeScore(EvaluationModel evaluation)
        {
            if (evaluation != null)
            {
                float rate = 0;

                if (!string.IsNullOrEmpty(evaluation.RateEncode))
                {
                    rate = float.Parse(EncodeService.Decode(evaluation.RateEncode));
                }

                evaluation.Score = EncodeService.Decode(evaluation.ScoreEncode)
                    .Replace("_" + evaluation.KPIId, string.Empty);
                evaluation.Rate = rate;
            }
        }

        private void DecodeScore(IList<EvaluationModel> evaluations)
        {
            foreach (EvaluationModel evaluation in evaluations)
            {
                DecodeScore(evaluation);
            }
        }
    }
}