namespace SMS.EPA.Repositories.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Model.ScoreRatingType;
    using SMS.EPA.Repositories.Interfaces;

    public class ReportRepository : IReportRepository
    {
        private readonly ICryptographyService cryptographyService;

        public ReportRepository()
            : this(null)
        {
        }

        public ReportRepository(ICryptographyService cryptographyService)
        {
            this.cryptographyService = cryptographyService ?? new CryptographyService();
        }

        public EPASummaryModel GetEPASummary(ICriteria criteria)
        {
            EPASummaryModel epaSummary = new EPASummaryModel();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    epaSummary.PeriodSelecteds = multi.Read<PeriodModel>().ToList();

                    epaSummary.EPAInfos = multi.Read<EPASummaryEmployeeModel, Group, JobTitle, EPASummaryEmployeeModel>(
                        (res, gr, job) =>
                        {
                            res.Group = gr;
                            res.JobTitle = job;

                            return res;
                        },
                        "GroupId, JobTitleId").ToList();

                    IList<EPASummaryPeriodModel> periods = multi.Read<EPASummaryPeriodModel, EPAStatusModel, EPASummaryPeriodModel>(
                        (res, status) =>
                        {
                            res.Status = status;

                            return res;
                        },
                        "StatusId").ToList();

                    var hashParam = db.GetOuputParams();
                    var totalItem = (int)hashParam["totalitem"];

                    epaSummary.TotalItem = totalItem;

                    GroupPeriodByEmployee(epaSummary.EPAInfos, periods);
                    DecodeFinalScoreAndGrade(epaSummary.EPAInfos);
                }
            }

            return epaSummary;
        }

        public EPADetailModel GetEPADetail(ICriteria criteria)
        {
            EPADetailModel epaDetail = new EPADetailModel();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    epaDetail.PhaseSelecteds = multi.Read<EvaluationPhaseModel>();

                    epaDetail.MemberEPAs = multi.Read<EPAModel, EPAEmployeeModel, EPAStatusModel, EPAPeriodModel, ScoreRatingTypeModel, EPAModel>(
                    (info, employee, status, period, scoreRatingType) =>
                    {
                        info.Employee = employee;
                        info.Status = status;
                        info.Period = period;
                        info.ScoreRatingType = scoreRatingType;

                        return info;
                    },
                    "EmployeeId, StatusId, PeriodId, ScoreRatingTypeId");

                    IEnumerable<EPAProcessModel> processes = multi.Read<EPAProcessModel>();

                    var hashParam = db.GetOuputParams();
                    var totalItem = (int)hashParam["totalitem"];

                    epaDetail.TotalItem = totalItem;

                    GroupEPAProcessByEPA(epaDetail.MemberEPAs, processes);
                    DecodeAllScore(epaDetail.MemberEPAs);
                }
            }

            return epaDetail;
        }

        public EPADetailModel GetMonthlyEPADetail(ICriteria criteria)
        {
            EPADetailModel epaDetail = new EPADetailModel();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    epaDetail.PhaseSelecteds = multi.Read<EvaluationPhaseModel>();

                    epaDetail.MemberEPAs = multi.Read<EPAModel, EPAEmployeeModel, EPAStatusModel, EPAPeriodModel, EPAModel>(
                    (info, employee, status, period) =>
                    {
                        info.Employee = employee;
                        info.Status = status;
                        info.Period = period;

                        return info;
                    },
                    "EmployeeId, StatusId, PeriodId");

                    IEnumerable<EPAProcessModel> processes = multi.Read<EPAProcessModel>();

                    var hashParam = db.GetOuputParams();
                    var totalItem = (int)hashParam["totalitem"];

                    epaDetail.TotalItem = totalItem;

                    GroupEPAProcessByEPA(epaDetail.MemberEPAs, processes);
                    DecodeAllScore(epaDetail.MemberEPAs);
                }
            }

            return epaDetail;
        }

        public EPASummaryModel GetMonthlyEPASummary(ICriteria criteria)
        {
            EPASummaryModel epaSummary = new EPASummaryModel();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    epaSummary.PeriodSelecteds = multi.Read<PeriodModel>().ToList();

                    epaSummary.EPAInfos = multi.Read<EPASummaryEmployeeModel, Group, JobTitle, EPASummaryEmployeeModel>(
                        (res, gr, job) =>
                        {
                            res.Group = gr;
                            res.JobTitle = job;

                            return res;
                        },
                        "GroupId, JobTitleId").ToList();

                    IList<EPASummaryPeriodModel> periods = multi.Read<EPASummaryPeriodModel, EPAStatusModel, EPASummaryPeriodModel>(
                        (res, status) =>
                        {
                            res.Status = status;

                            return res;
                        },
                        "StatusId").ToList();

                    var hashParam = db.GetOuputParams();
                    var totalItem = (int)hashParam["totalitem"];

                    epaSummary.TotalItem = totalItem;

                    GroupPeriodByEmployee(epaSummary.EPAInfos, periods);
                    DecodeFinalScoreAndGrade(epaSummary.EPAInfos);
                }
            }

            return epaSummary;
        }

        private void GroupPeriodByEmployee(IEnumerable<EPASummaryEmployeeModel> epaSummaries, IList<EPASummaryPeriodModel> periods)
        {
            foreach (EPASummaryEmployeeModel epaSummary in epaSummaries)
            {
                foreach (EPASummaryPeriodModel period in periods.Where(x => epaSummary.EmployeeId.Equals(x.EmployeeId)))
                {
                    epaSummary.Periods.Add(period);
                }
            }
        }

        private void GroupEPAProcessByEPA(IEnumerable<EPAModel> epaInfos, IEnumerable<EPAProcessModel> processes)
        {
            foreach (EPAModel epaInfo in epaInfos)
            {
                foreach (EPAProcessModel process in processes.Where(x => x.EPAId == epaInfo.EPAId))
                {
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
                    }
                }
            }
        }

        private void DecodeFinalScoreAndGrade(IEnumerable<EPASummaryEmployeeModel> epaSummaries)
        {
            foreach (EPASummaryPeriodModel period in epaSummaries.SelectMany(x => x.Periods))
            {
                period.FinalScore = cryptographyService.Decrypt(period.FinalScoreEncrypt);
                period.Grade = cryptographyService.Decrypt(period.GradeEncrypt);
            }
        }

        private void DecodeAllScore(IEnumerable<EPAModel> epaInfos)
        {
            foreach (EPAModel epaInfo in epaInfos)
            {
                epaInfo.FinalScore = cryptographyService.Decrypt(epaInfo.FinalScoreEncrypt);
                epaInfo.Grade = cryptographyService.Decrypt(epaInfo.GradeEncrypt);
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
                process.TotalScore = cryptographyService.Decrypt(process.TotalScoreEncrypt);
                process.AdjustScore = cryptographyService.Decrypt(process.AdjustScoreEncrypt);
            }
        }

        private void DecodeTotalScore(IList<EPAProcessModel> processes)
        {
            foreach (EPAProcessModel process in processes)
            {
                DecodeTotalScore(process);
            }
        }
    }
}