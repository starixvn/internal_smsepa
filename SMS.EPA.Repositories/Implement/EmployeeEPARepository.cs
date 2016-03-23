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
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.EmployeeEPA.RatingByKPI;
    using SMS.EPA.Model.Enitites;
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
                    IEnumerable<EPAModel> epaInfos = multi.Read<EPAModel, RatingTypeModel, StatusModel, EPAPeriodModel, ScoreRatingTypeModel, EPAModel>(
                    (epaInfo, ratingType, status, period, scoreRatingType) =>
                    {
                        epaInfo.RatingType = ratingType;
                        epaInfo.ScoreRatingType = scoreRatingType;
                        epaInfo.Status = status;
                        epaInfo.Period = period;

                        return epaInfo;
                    },
                    "RatingTypeId, StatusId, PeriodId, ScoreRatingTypeId");

                    IEnumerable<EPAEmployeeModel> employees = multi.Read<EPAEmployeeModel, DepartmentModel, Group, JobTitle, EPAEmployeeModel>(
                        (epaInfo, dept, gr, job) =>
                        {
                            epaInfo.Department = dept;
                            epaInfo.Group = gr;
                            epaInfo.JobTitle = job;

                            return epaInfo;
                        },
                        "DepartmentId, GroupId, JobTitleId");

                    IEnumerable<EPAProcessModel> processes = multi.Read<EPAProcessModel>();

                    GroupEmployeeByEPA(epaInfos, employees);
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

                    epaInfo = multi.Read<EPAModel, RatingTypeModel, StatusModel, EPAPeriodModel, ScoreRatingTypeModel, EPAModel>(
                    (info, ratingType, status, period, scoreRatingType) =>
                    {
                        info.RatingType = ratingType;
                        info.ScoreRatingType = scoreRatingType;
                        info.Status = status;
                        info.Period = period;

                        return info;
                    },
                    "RatingTypeId, StatusId, PeriodId, ScoreRatingTypeId").FirstOrDefault();

                    if (epaInfo != null)
                    {
                        epaInfo.Employee = multi.Read<EPAEmployeeModel, DepartmentModel, Group, JobTitle, EPAEmployeeModel>(
                            (epa, dept, gr, job) =>
                            {
                                epa.Department = dept;
                                epa.Group = gr;
                                epa.JobTitle = job;

                                return epa;
                            },
                            "DepartmentId, GroupId, JobTitleId").FirstOrDefault();

                        IEnumerable<EPAProcessModel> processes = multi.Read<EPAProcessModel>();

                        epaInfo.KPIGroups = multi.Read<EPAKPIGroupModel>().ToList();

                        IEnumerable<EPAKPIModel> kpis = multi.Read<EPAKPIModel>();

                        IEnumerable<EvaluationModel> evaluations = multi.Read<EvaluationModel>();

                        IList<EPAProcessModel> evaluators = multi.Read<EPAProcessModel>().ToList();

                        epaInfo.LastProcesses = multi.Read<EPAProcessModel>().ToList();

                        GroupEvaluatorByProcess(epaInfo, evaluators);
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
                    epaInfos = multi.Read<RatingByKPIModel, RatingTypeModel, TemporaryKPIGroupModel, RatingByKPIModel>(
                        (epaInfo, type, kgr) =>
                        {
                            epaInfo.RatingType = type;
                            epaInfo.KPIGroup = kgr;

                            return epaInfo;
                        },
                        "RatingTypeId, KPIGroupId");

                    IEnumerable<EPAEmployeeModel> employees = multi.Read<EPAEmployeeModel, DepartmentModel, Group, JobTitle, EPAEmployeeModel>(
                        (epaInfo, dept, gr, job) =>
                        {
                            epaInfo.Department = dept;
                            epaInfo.Group = gr;
                            epaInfo.JobTitle = job;

                            return epaInfo;
                        },
                        "DepartmentId, GroupId, JobTitleId");

                    IEnumerable<TemporaryKPIModel> kpis = multi.Read<TemporaryKPIModel, TemporaryKPIGroupModel, TemporaryKPIModel>(
                        (kpi, kgr) =>
                        {
                            kpi.KPIGroup = kgr;

                            return kpi;
                        },
                        "KPIGroupId");

                    GroupEmployeeByEPA(epaInfos, employees);
                    GroupKPIByEPA(epaInfos, kpis);
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
                    epaInfos = multi.Read<RatingByKPIModel, RatingTypeModel, TemporaryKPIGroupModel, RatingByKPIModel>(
                        (epaInfo, type, kgr) =>
                        {
                            epaInfo.RatingType = type;
                            epaInfo.KPIGroup = kgr;

                            return epaInfo;
                        },
                        "RatingTypeId, KPIGroupId");

                    IEnumerable<EPAEmployeeModel> employees = multi.Read<EPAEmployeeModel, DepartmentModel, Group, JobTitle, EPAEmployeeModel>(
                        (epaInfo, dept, gr, job) =>
                        {
                            epaInfo.Department = dept;
                            epaInfo.Group = gr;
                            epaInfo.JobTitle = job;

                            return epaInfo;
                        },
                        "DepartmentId, GroupId, JobTitleId");

                    IEnumerable<TemporaryKPIModel> kpis = multi.Read<TemporaryKPIModel, TemporaryKPIGroupModel, TemporaryKPIModel>(
                        (kpi, kgr) =>
                        {
                            kpi.KPIGroup = kgr;

                            return kpi;
                        },
                        "KPIGroupId");

                    GroupEmployeeByEPA(epaInfos, employees);
                    GroupKPIByEPA(epaInfos, kpis);
                    DecodeAllScore(epaInfos);
                }
            }

            return epaInfos;
        }

        #endregion Rating By KPI

        private void GroupEmployeeByEPA(IEnumerable<EPAModel> epaInfos, IEnumerable<EPAEmployeeModel> employees)
        {
            foreach (EPAModel epaInfo in epaInfos)
            {
                foreach (EPAEmployeeModel employee in employees)
                {
                    if (employee.EPAId.Equals(epaInfo.EPAId))
                    {
                        epaInfo.Employee = employee;
                        break;
                    }
                }
            }
        }

        private void GroupEPAProcessByEPA(EPAModel epaInfo, IEnumerable<EPAProcessModel> processes)
        {
            foreach (EPAProcessModel process in processes)
            {
                if (process.EPAId.Equals(epaInfo.EPAId))
                {
                    switch (process.ProcessId)
                    {
                        case (int)ProcessType.Self:
                            epaInfo.ProcessBySelf = process;
                            break;

                        case (int)ProcessType.Evaluator:
                            epaInfo.ProcessByEvaluators.Add(process);
                            break;

                        case (int)ProcessType.Reviewer:
                            epaInfo.ProcessByReviewers.Add(process);
                            break;

                        case (int)ProcessType.Management:
                            epaInfo.ProcessByManagements.Add(process);
                            break;
                    }
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

        private void GroupEvaluatorByProcess(EPAModel epaInfo, EPAProcessModel evaluator)
        {
            switch (evaluator.ProcessId)
            {
                case (int)ProcessType.HumanResource:
                    epaInfo.HumanResources.Add(evaluator);
                    break;

                case (int)ProcessType.Evaluator:
                    epaInfo.Evaluators.Add(evaluator);
                    break;

                case (int)ProcessType.Reviewer:
                    epaInfo.Reviewers.Add(evaluator);
                    break;

                case (int)ProcessType.Management:
                    epaInfo.Managements.Add(evaluator);
                    break;
            }
        }

        private void GroupEvaluatorByProcess(EPAModel epaInfo, IEnumerable<EPAProcessModel> evaluators)
        {
            foreach (EPAProcessModel evaluator in evaluators)
            {
                if (evaluator.EPAId.Equals(epaInfo.EPAId))
                {
                    GroupEvaluatorByProcess(epaInfo, evaluator);
                }
            }
        }

        private void GroupKPIByKPIGroup(IList<EPAKPIGroupModel> kpiGroups, IEnumerable<EPAKPIModel> kpis)
        {
            foreach (EPAKPIGroupModel kpiGroup in kpiGroups)
            {
                foreach (EPAKPIModel kpi in kpis)
                {
                    if (kpi.EPAId.Equals(kpiGroup.EPAId) && kpi.KPIGroupId.Equals(kpiGroup.KPIGroupId))
                    {
                        kpiGroup.KPIs.Add(kpi);
                    }
                }
            }
        }

        private void GroupEvaluationOnEachKPI(IEnumerable<EPAKPIModel> kpis, IEnumerable<EvaluationModel> evaluations)
        {
            foreach (EPAKPIModel kpi in kpis)
            {
                foreach (EvaluationModel evaluation in evaluations)
                {
                    if (evaluation.EPAId.Equals(kpi.EPAId) && evaluation.KPIId.Equals(kpi.KPIId))
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
        }

        private void GroupEmployeeByEPA(IEnumerable<RatingByKPIModel> epaInfos, IEnumerable<EPAEmployeeModel> employees)
        {
            foreach (RatingByKPIModel epaInfo in epaInfos)
            {
                foreach (EPAEmployeeModel employee in employees)
                {
                    if (employee.EPAId.Equals(epaInfo.EPAId))
                    {
                        epaInfo.Employee = employee;
                        break;
                    }
                }
            }
        }

        private void GroupKPIByEPA(IEnumerable<RatingByKPIModel> epaInfos, IEnumerable<TemporaryKPIModel> kpis)
        {
            foreach (RatingByKPIModel epaInfo in epaInfos)
            {
                foreach (TemporaryKPIModel kpi in kpis)
                {
                    if (kpi.EPAId.Equals(epaInfo.EPAId) && kpi.KPIGroup.KPIGroupId.Equals(epaInfo.KPIGroup.KPIGroupId))
                    {
                        epaInfo.KPIGroup.KPIs.Add(kpi);
                    }
                }
            }
        }

        private void DecodeAllScore(EPAModel epaInfo)
        {
            epaInfo.LastFinalScore = Decode(epaInfo.LastFinalScoreEncode);
            epaInfo.LastGrade = Decode(epaInfo.LastGradeEncode);
            epaInfo.FinalScore = Decode(epaInfo.FinalScoreEncode);
            epaInfo.Grade = Decode(epaInfo.GradeEncode);
            DecodeTotalScore(epaInfo.ProcessBySelf);
            DecodeTotalScore(epaInfo.ProcessByEvaluators);
            DecodeTotalScore(epaInfo.ProcessByReviewers);
            DecodeTotalScore(epaInfo.ProcessByManagements);
            DecodeTotalScore(epaInfo.LastProcesses);

            foreach (EPAKPIGroupModel kpiGroup in epaInfo.KPIGroups)
            {
                foreach (EPAKPIModel kpi in kpiGroup.KPIs)
                {
                    DecodeScore(kpi.EvaluationBySelf);
                    DecodeScore(kpi.EvaluationByEvaluators);
                }
            }
        }

        private void DecodeAllScore(IEnumerable<RatingByKPIModel> epaInfos)
        {
            foreach (RatingByKPIModel epaInfo in epaInfos)
            {
                epaInfo.TotalScore = Decode(epaInfo.TotalScoreEncode);

                foreach (TemporaryKPIModel kpi in epaInfo.KPIGroup.KPIs)
                {
                    float rate = 0;

                    if (!string.IsNullOrEmpty(kpi.RateEncode))
                    {
                        rate = float.Parse(Decode(kpi.RateEncode));
                    }

                    kpi.Score = Decode(kpi.ScoreEncode).Replace("_" + kpi.KPIId, string.Empty);
                    kpi.Rate = rate;
                }
            }
        }

        private void DecodeTotalScore(EPAProcessModel process)
        {
            process.TotalScore = Decode(process.TotalScoreEncode);
            process.AdjustScore = Decode(process.AdjustScoreEncode);
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
            float rate = 0;

            if (!string.IsNullOrEmpty(evaluation.RateEncode))
            {
                rate = float.Parse(Decode(evaluation.RateEncode));
            }

            evaluation.Score = Decode(evaluation.ScoreEncode).Replace("_" + evaluation.KPIId, string.Empty);
            evaluation.Rate = rate;
        }

        private void DecodeScore(IList<EvaluationModel> evaluations)
        {
            foreach (EvaluationModel evaluation in evaluations)
            {
                DecodeScore(evaluation);
            }
        }

        private string Decode(string cipherText)
        {
            return !string.IsNullOrEmpty(cipherText) ? EncodeService.Decode(cipherText) : string.Empty;
        }
    }
}