namespace SMS.EPA.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Enumerations;
    using Core.Services;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;
    using Model.Enitites;
    using Model.Report.EPASummary;

    public class ReportRepository : IReportRepository
    {
        private readonly ICryptographyService _cryptographyService;

        public ReportRepository()
            : this(null)
        {
        }

        public ReportRepository(ICryptographyService cryptographyService)
        {
            _cryptographyService = cryptographyService ?? new CryptographyService();
        }

        public EPASummaryModel GetEPASummary(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var epaSummary = new EPASummaryModel();

                    epaSummary.PeriodSelecteds = multi.Read<PeriodModel>().ToList();

                    epaSummary.EPAInfos = multi.Read<EPASummaryEmployeeModel, Group, JobTitle, EPASummaryEmployeeModel>(
                        (res, gr, job) =>
                        {
                            res.Group = gr;
                            res.JobTitle = job;

                            return res;
                        },
                        "GroupId, JobTitleId").ToList();

                    IList<EPASummaryPeriodModel> periods = multi.Read<EPASummaryPeriodModel>().ToList();

                    if (!multi.IsEmptyOrClosed)
                    {
                        // If not montly Period
                        IList<EPASummaryDetailScoreModel> detailScores = multi.Read<EPASummaryDetailScoreModel>().ToList();
                        DecodeDetailScores(detailScores);
                        AddDetailScoresToPeriods(periods, detailScores);
                    }

                    GroupPeriodByEmployee(epaSummary.EPAInfos, periods);
                    DecodeFinalScoreAndGrade(epaSummary.EPAInfos);

                    return epaSummary;
                }
            }
        }

        public EPADetailModel GetEPADetail(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var epaDetail = new EPADetailModel();

                    epaDetail.PhaseSelecteds = multi.Read<EvaluationPhaseModel>();

                    epaDetail.EpaInfos = multi.Read<EPAModel, EPAEmployeeModel, EPAPeriodModel, ScoreRatingTypeModel, EPAModel>(
                            (info, employee, period, scoreRatingType) =>
                            {
                                info.Employee = employee;
                                info.Period = period;
                                info.ScoreRatingType = scoreRatingType;

                                return info;
                            },
                            "EmployeeId, PeriodId, ScoreRatingTypeId");

                    var processes = multi.Read<EPAProcessModel, EPAEmployeeModel, EPAProcessModel>(
                        (process, employee) =>
                        {
                            process.EvaluatorInfo = employee;

                            return process;
                        },
                        "EvaluatorId");

                    GroupEPAProcessByEPA(epaDetail.EpaInfos, processes);
                    DecodeAllScore(epaDetail.EpaInfos);

                    return epaDetail;
                }
            }
        }

        public EPADetailModel GetMonthlyEPADetail(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var epaDetail = new EPADetailModel();

                    epaDetail.PhaseSelecteds = multi.Read<EvaluationPhaseModel>();

                    epaDetail.EpaInfos = multi.Read<EPAModel, EPAEmployeeModel, EPAPeriodModel, EPAModel>(
                        (info, employee, period) =>
                        {
                            info.Employee = employee;
                            info.Period = period;

                            return info;
                        },
                        "EmployeeId, PeriodId");

                    IEnumerable<EPAProcessModel> processes = multi.Read<EPAProcessModel>();

                    GroupEPAProcessByEPA(epaDetail.EpaInfos, processes);
                    DecodeAllScore(epaDetail.EpaInfos);

                    return epaDetail;
                }
            }
        }

        private static void AddDetailScoresToPeriods(IList<EPASummaryPeriodModel> periods, IList<EPASummaryDetailScoreModel> detailScores)
        {
            foreach (EPASummaryPeriodModel summaryPeriodModel in periods)
            {
                summaryPeriodModel.DetailScores = detailScores
                    .Where(x => x.EmployeeId == summaryPeriodModel.EmployeeId && x.PeriodId == summaryPeriodModel.PeriodId)
                    .ToList();
            }
        }

        private static void GroupPeriodByEmployee(
            IEnumerable<EPASummaryEmployeeModel> epaSummaries,
            IList<EPASummaryPeriodModel> periods)
        {
            foreach (EPASummaryEmployeeModel epaSummary in epaSummaries)
            {
                foreach (EPASummaryPeriodModel period in periods.Where(x => epaSummary.EmployeeId.Equals(x.EmployeeId)))
                {
                    epaSummary.Periods.Add(period);
                }
            }
        }

        private static void GroupEPAProcessByEPA(IEnumerable<EPAModel> epaInfos, IEnumerable<EPAProcessModel> processes)
        {
            foreach (EPAModel epaInfo in epaInfos)
            {
                foreach (EPAProcessModel process in processes.Where(x => x.EPAId == epaInfo.EPAId))
                {
                    epaInfo.AllProcesses.Add(process);

                    switch (process.ProcessId)
                    {
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
                            continue;
                    }
                }
            }
        }

        private void DecodeFinalScoreAndGrade(IEnumerable<EPASummaryEmployeeModel> epaSummaries)
        {
            foreach (EPASummaryPeriodModel period in epaSummaries.SelectMany(x => x.Periods))
            {
                period.FinalScore = _cryptographyService.Decrypt(period.FinalScore);
                period.Grade = _cryptographyService.Decrypt(period.Grade);
            }
        }

        private void DecodeAllScore(IEnumerable<EPAModel> epaInfos)
        {
            foreach (EPAModel epaInfo in epaInfos)
            {
                epaInfo.FinalScore = _cryptographyService.Decrypt(epaInfo.FinalScore);
                epaInfo.Grade = _cryptographyService.Decrypt(epaInfo.Grade);
                DecodeTotalScore(epaInfo.Process.Self);
                DecodeTotalScore(epaInfo.Process.Evaluators);
                DecodeTotalScore(epaInfo.Process.Reviewers);
                DecodeTotalScore(epaInfo.Process.Managements);
            }
        }

        private void DecodeTotalScore(EPAProcessModel process)
        {
            if (process != null)
            {
                process.TotalScore = _cryptographyService.Decrypt(process.TotalScore);
                process.AdjustScore = _cryptographyService.Decrypt(process.AdjustScore);
            }
        }

        private void DecodeTotalScore(IList<EPAProcessModel> processes)
        {
            foreach (EPAProcessModel process in processes)
            {
                DecodeTotalScore(process);
            }
        }

        private void DecodeDetailScores(IList<EPASummaryDetailScoreModel> detailScores)
        {
            foreach (EPASummaryDetailScoreModel detailScore in detailScores)
            {
                detailScore.AdjustScore = _cryptographyService.Decrypt(detailScore.AdjustScore);
                detailScore.TotalScore = _cryptographyService.Decrypt(detailScore.TotalScore);
            }
        }
    }
}