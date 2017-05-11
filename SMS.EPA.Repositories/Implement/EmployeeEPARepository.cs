namespace SMS.EPA.Repositories
{
    using Core.Enumerations;
    using Core.Services;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    public class EmployeeEPARepository : IEmployeeEPARepository
    {
        private readonly ICryptographyService cryptographyService;

        public EmployeeEPARepository()
            : this(null)
        {
        }

        public EmployeeEPARepository(ICryptographyService cryptographyService)
        {
            this.cryptographyService = cryptographyService ?? new CryptographyService();
        }

        public IEnumerable<EPAModel> GetEPA(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReaderWrapper multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IEnumerable<EPAModel> epaInfos = multi
                        .Read<EPAModel, EPAEmployeeModel, RatingTypeModel, EPAPeriodModel, ScoreRatingTypeModel, EPAModel>(
                        (epaInfo, employee, ratingType, period, scoreRatingType) =>
                            {
                                epaInfo.Employee = employee;
                                epaInfo.RatingType = ratingType;
                                epaInfo.ScoreRatingType = scoreRatingType;
                                epaInfo.Period = period;

                                return epaInfo;
                            },
                            "EmployeeId, RatingTypeId, PeriodId, ScoreRatingTypeId");

                    IEnumerable<EPAProcessModel> processes =
                        multi.Read<EPAProcessModel, EPAEmployeeModel, EPAEmployeeModel, EPAProcessModel>(
                            (process, evaluator, modifiedBy) =>
                            {
                                process.EvaluatorInfo = evaluator;
                                process.ModifiedBy = modifiedBy;

                                return process;
                            },
                            "EvaluatorId, ModifiedById");

                    GroupEPAProcessByEPA(epaInfos, processes);

                    foreach (EPAModel epaInfo in epaInfos)
                    {
                        DecryptAllScore(epaInfo);
                    }

                    return epaInfos;
                }
            }
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Need to review here.")]
        public EPAModel GetEPAInfo(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReaderWrapper multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var epaInfo = multi.Read<EPAModel, EPAEmployeeModel, RatingTypeModel, EPAPeriodModel, ScoreRatingTypeModel, EPAModel>(
                            (info, employee, ratingType, period, scoreRatingType) =>
                            {
                                info.Employee = employee;
                                info.RatingType = ratingType;
                                info.ScoreRatingType = scoreRatingType;
                                info.Period = period;

                                return info;
                            },
                            "EmployeeId, RatingTypeId, PeriodId, ScoreRatingTypeId").FirstOrDefault();

                    if (epaInfo != null)
                    {
                        IEnumerable<EPAProcessModel> processes = multi.Read<EPAProcessModel, EPAEmployeeModel, EPAEmployeeModel, EPAProcessModel>(
                                (process, evaluator, modifiedBy) =>
                                {
                                    process.EvaluatorInfo = evaluator;
                                    process.ModifiedBy = modifiedBy;

                                    return process;
                                },
                                "EvaluatorId, ModifiedById");

                        epaInfo.KPIGroups = multi.Read<EPAKpiGroupModel>().ToList();

                        IEnumerable<EPAKpiModel> kpis = multi.Read<EPAKpiModel>();

                        IEnumerable<EvaluationModel> evaluations = multi.Read<EvaluationModel>();

                        epaInfo.LastProcesses = multi.Read<EPAProcessModel>().ToList();

                        GroupEPAProcessByEPA(epaInfo, processes);
                        GroupKPIByKPIGroup(epaInfo.KPIGroups, kpis);

                        foreach (EPAKpiGroupModel kpiGroup in epaInfo.KPIGroups)
                        {
                            GroupEvaluationOnEachKPI(kpiGroup.KPIs, evaluations);
                        }

                        DecryptAllScore(epaInfo);
                    }

                    return epaInfo;
                }
            }
        }

        public IEnumerable<EPAModel> GetEPAEndEvaluationPeriod(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReaderWrapper multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IEnumerable<EPAModel> epaInfos = multi.Read<EPAModel, EPAEmployeeModel, EPAPeriodModel, EPAModel>(
                        (epaInfo, emp, period) =>
                        {
                            epaInfo.Employee = emp;
                            epaInfo.Period = period;

                            return epaInfo;
                        },
                        "EmployeeId, PeriodId");

                    IEnumerable<EPAProcessModel> processes =
                        multi.Read<EPAProcessModel, EPAEmployeeModel, EPAEmployeeModel, EPAProcessModel>(
                            (process, evaluator, modifiedBy) =>
                            {
                                process.EvaluatorInfo = evaluator;
                                process.ModifiedBy = modifiedBy;

                                return process;
                            },
                            "EvaluatorId, ModifiedById");

                    GroupEPAProcessByEPA(epaInfos, processes);

                    foreach (EPAModel epaInfo in epaInfos)
                    {
                        DecryptAllScore(epaInfo);
                    }

                    return epaInfos;
                }
            }
        }

        public IList<EPAModel> GetEPAToUpdateFinalScore(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReaderWrapper multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IList<EPAModel> epaInfos = multi.Read<EPAModel, EPAEmployeeModel, EPAPeriodModel, EPAModel>(
                            (epaInfo, emp, period) =>
                            {
                                epaInfo.Employee = emp;
                                epaInfo.Period = period;

                                return epaInfo;
                            },
                            "EmployeeId, PeriodId")
                        .ToList();

                    IEnumerable<EPAProcessModel> processes =
                        multi.Read<EPAProcessModel, EPAEmployeeModel, EPAEmployeeModel, EPAProcessModel>(
                            (process, evaluator, modifiedBy) =>
                            {
                                process.EvaluatorInfo = evaluator;
                                process.ModifiedBy = modifiedBy;

                                return process;
                            },
                            "EvaluatorId, ModifiedById");

                    GroupEPAProcessByEPA(epaInfos, processes);

                    foreach (EPAModel epaInfo in epaInfos)
                    {
                        DecryptAllScore(epaInfo);
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
                using (GridReaderWrapper multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    result.MyEpaInfos = multi.Read<EPAModel, EPAPeriodModel, EPAModel>(
                        (epa, period) =>
                        {
                            epa.Period = period;

                            return epa;
                        },
                        "PeriodId");

                    IEnumerable<EPAProcessModel> processes =
                        multi.Read<EPAProcessModel, EPAEmployeeModel, EPAEmployeeModel, EPAProcessModel>(
                            (process, evaluator, modifiedBy) =>
                            {
                                process.EvaluatorInfo = evaluator;
                                process.ModifiedBy = modifiedBy;

                                return process;
                            },
                            "EvaluatorId, ModifiedById");

                    GroupEPAProcessByEPA(result.MyEpaInfos, processes);

                    foreach (EPAModel epaInfo in result.MyEpaInfos)
                    {
                        DecryptAllScore(epaInfo);
                    }
                }
            }

            return result;
        }

        public EPARecommendationModel GetRecommendationById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<EPARecommendationModel, JobGradeModel, EPARecommendationModel>(
                        (recommendation, job) =>
                        {
                            recommendation.JobGrade = job;

                            return recommendation;
                        },
                        "JobGradeId",
                        criteria.GetSpParams()).FirstOrDefault();
            }
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
            IEnumerable<RatingByKPIModel> epaInfos;
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReaderWrapper multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    epaInfos = multi
                        .Read<RatingByKPIModel, EPAEmployeeModel, RatingTypeModel, TemporaryKPIGroupModel, RatingByKPIModel>(
                            (epaInfo, employee, type, kgr) =>
                            {
                                epaInfo.Employee = employee;
                                epaInfo.RatingType = type;
                                epaInfo.KPIGroup = kgr;
                                return epaInfo;
                            },
                            "EmployeeId, RatingTypeId, KPIGroupId");

                    IEnumerable<TemporaryKPIModel> kpis = multi
                        .Read<TemporaryKPIModel, TemporaryKPIGroupModel, TemporaryKPIModel>(
                            (kpi, kgr) =>
                            {
                                kpi.KPIGroup = kgr;
                                return kpi;
                            },
                            "KPIGroupId");

                    IEnumerable<TemporaryKPIModel> kpisBySelf =
                        multi.Read<TemporaryKPIModel, TemporaryKPIGroupModel, TemporaryKPIModel>(
                            (kpi, kgr) =>
                            {
                                kpi.KPIGroup = kgr;
                                return kpi;
                            },
                            "KPIGroupId");

                    GroupKPIByEPA(epaInfos, kpis);
                    GroupKPIBySelf(epaInfos, kpisBySelf);
                    DecryptAllScore(epaInfos);
                }
            }

            return epaInfos;
        }

        #endregion Rating By KPI

        private static void GroupEPAProcessByEPA(EPAModel epaInfo, IEnumerable<EPAProcessModel> processes)
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

                    default:
                        throw new ArgumentException("type not in case");
                }
            }
        }

        private static void GroupEPAProcessByEPA(IEnumerable<EPAModel> epaInfos, IEnumerable<EPAProcessModel> processes)
        {
            foreach (EPAModel epaInfo in epaInfos)
            {
                if (processes != null && processes.Any())
                {
                    GroupEPAProcessByEPA(epaInfo, processes);
                }
            }
        }

        private static void GroupKPIByKPIGroup(IList<EPAKpiGroupModel> kpiGroups, IEnumerable<EPAKpiModel> kpis)
        {
            foreach (EPAKpiGroupModel kpiGroup in kpiGroups)
            {
                IEnumerable<EPAKpiModel> tempkpis = kpis.Where(x => x.EPAId.Equals(kpiGroup.EPAId) && x.KPIGroupId.Equals(kpiGroup.KPIGroupId));
                foreach (EPAKpiModel kpi in tempkpis)
                {
                    kpiGroup.KPIs.Add(kpi);
                }
            }
        }

        private static void GroupEvaluationOnEachKPI(IEnumerable<EPAKpiModel> kpis, IEnumerable<EvaluationModel> evaluations)
        {
            foreach (EPAKpiModel kpi in kpis)
            {
                IEnumerable<EvaluationModel> tempEvaluations = evaluations.Where(x => x.EPAId.Equals(kpi.EPAId) && x.KPIId.Equals(kpi.KPIId));
                foreach (EvaluationModel evaluation in tempEvaluations)
                {
                    switch (evaluation.ProcessId)
                    {
                        case (int)ProcessType.Self:
                            kpi.EvaluationBySelf = evaluation;
                            break;

                        case (int)ProcessType.Evaluator:
                            kpi.EvaluationByEvaluators.Add(evaluation);
                            break;

                        default:
                            throw new ArgumentException("type not in case");
                    }
                }
            }
        }

        private static void GroupKPIByEPA(IEnumerable<RatingByKPIModel> epaInfos, IEnumerable<TemporaryKPIModel> kpis)
        {
            foreach (RatingByKPIModel epaInfo in epaInfos)
            {
                IEnumerable<TemporaryKPIModel> tempkpis =
                    kpis.Where(x => x.EPAId.Equals(epaInfo.EPAId) && x.KPIGroup.KPIGroupId.Equals(epaInfo.KPIGroup.KPIGroupId));
                foreach (TemporaryKPIModel kpi in tempkpis)
                {
                    epaInfo.KPIGroup.KPIs.Add(kpi);
                }
            }
        }

        private static void GroupKPIBySelf(IEnumerable<RatingByKPIModel> epaInfos, IEnumerable<TemporaryKPIModel> kpis)
        {
            foreach (RatingByKPIModel epaInfo in epaInfos)
            {
                IEnumerable<TemporaryKPIModel> tempkpis =
                    kpis.Where(x => x.EPAId.Equals(epaInfo.EPAId) && x.KPIGroup.KPIGroupId.Equals(epaInfo.KPIGroup.KPIGroupId));
                foreach (TemporaryKPIModel kpi in tempkpis)
                {
                    epaInfo.KPIGroup.KPIsBySelf.Add(kpi);
                }
            }
        }

        private void DecryptAllScore(EPAModel epaInfo)
        {
            epaInfo.LastFinalScore = cryptographyService.Decrypt(epaInfo.LastFinalScore);
            epaInfo.LastGrade = cryptographyService.Decrypt(epaInfo.LastGrade);
            epaInfo.FinalScore = cryptographyService.Decrypt(epaInfo.FinalScore);
            epaInfo.Grade = cryptographyService.Decrypt(epaInfo.Grade);
            epaInfo.OldFinalScore = cryptographyService.Decrypt(epaInfo.OldFinalScore);
            DecryptTotalScore(epaInfo.Process.Self);
            DecryptTotalScore(epaInfo.Process.Evaluators);
            DecryptTotalScore(epaInfo.Process.Reviewers);
            DecryptTotalScore(epaInfo.Process.Managements);
            DecryptTotalScore(epaInfo.LastProcesses);

            foreach (EPAKpiModel kpi in epaInfo.KPIGroups.SelectMany(x => x.KPIs))
            {
                DecryptScore(kpi.EvaluationBySelf);
                DecryptScore(kpi.EvaluationByEvaluators);
            }
        }

        private void DecryptAllScore(IEnumerable<RatingByKPIModel> epaInfos)
        {
            foreach (RatingByKPIModel epaInfo in epaInfos)
            {
                epaInfo.TotalScore = cryptographyService.Decrypt(epaInfo.TotalScore);

                foreach (TemporaryKPIModel kpi in epaInfo.KPIGroup.KPIs)
                {
                    float rate = 0;

                    if (!string.IsNullOrEmpty(kpi.RateEncrypt))
                    {
                        rate = float.Parse(cryptographyService.Decrypt(kpi.RateEncrypt));
                    }

                    kpi.Score = cryptographyService.Decrypt(kpi.Score).Replace("_" + kpi.KPIId, string.Empty);
                    kpi.Rate = rate;
                }

                foreach (TemporaryKPIModel kpi in epaInfo.KPIGroup.KPIsBySelf)
                {
                    float rate = 0;

                    if (!string.IsNullOrEmpty(kpi.RateEncrypt))
                    {
                        rate = float.Parse(cryptographyService.Decrypt(kpi.RateEncrypt));
                    }

                    kpi.Score = cryptographyService.Decrypt(kpi.Score).Replace("_" + kpi.KPIId, string.Empty);
                    kpi.Rate = rate;
                }
            }
        }

        private void DecryptTotalScore(EPAProcessModel process)
        {
            if (process != null)
            {
                process.TotalScore = cryptographyService.Decrypt(process.TotalScore);
                process.AdjustScore = cryptographyService.Decrypt(process.AdjustScore);
            }
        }

        private void DecryptTotalScore(IList<EPAProcessModel> processes)
        {
            foreach (EPAProcessModel process in processes)
            {
                DecryptTotalScore(process);
            }
        }

        private void DecryptScore(EvaluationModel evaluation)
        {
            if (evaluation != null)
            {
                float rate = 0;

                if (!string.IsNullOrEmpty(evaluation.RateEncrypt))
                {
                    rate = float.Parse(cryptographyService.Decrypt(evaluation.RateEncrypt));
                }

                evaluation.Score = cryptographyService.Decrypt(evaluation.Score)
                    .Replace("_" + evaluation.KPIId, string.Empty);
                evaluation.Rate = rate;
            }
        }

        private void DecryptScore(IList<EvaluationModel> evaluations)
        {
            foreach (EvaluationModel evaluation in evaluations)
            {
                DecryptScore(evaluation);
            }
        }
    }
}