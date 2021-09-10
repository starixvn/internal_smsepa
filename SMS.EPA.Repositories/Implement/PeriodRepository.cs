namespace SMS.EPA.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Enumerations;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;

    public class PeriodRepository : IPeriodRepository
    {
        public IEnumerable<PeriodModel> GetPeriod(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<PeriodModel, TargetTypeModel, PeriodModel>(
                    (pe, tar) =>
                    {
                        pe.TargetType = tar;
                        return pe;
                    },
                    "TargetTypeId",
                    criteria.GetSpParams());
            }
        }

        public PeriodModel GetPeriodById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.QueryEntity<PeriodModel, TargetTypeModel, PeriodModel>(
                    (pe, tar) =>
                    {
                        pe.TargetType = tar;
                        return pe;
                    },
                    "TargetTypeId",
                    criteria.GetSpParams());
            }
        }

        public IEnumerable<PeriodEmployeeSettingModel> GetPeriodEmployeeSettingById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    var employeeSettings =
                        multi.Read<PeriodEmployeeSettingModel, EmployeeViewModel, DepartmentViewModel, Group, JobTitle, TargetTemplate, PeriodEmployeeSettingModel>(
                            (pe, emp, dep, gr, job, tar) =>
                            {
                                emp.Department = dep;
                                emp.Group = gr;
                                emp.JobTitle = job;
                                pe.Employee = emp;
                                pe.TargetTemplate = tar;

                                return pe;
                            },
                            "EmployeeId, DepartmentId, GroupId, JobTitleId, TargetTemplateId");

                    var flowSettings = multi.Read<PeriodFlowSettingModel, EvaluatorInfoViewModel, PeriodFlowSettingModel>(
                            (fl, eva) =>
                            {
                                fl.EvaluatorInfo = eva;
                                return fl;
                            },
                            "EvaluatorId");

                    foreach (PeriodEmployeeSettingModel employeeSetting in employeeSettings)
                    {
                        GetFlowSetting(flowSettings, employeeSetting);
                    }

                    return employeeSettings;
                }
            }
        }

        private static void GetFlowSetting(
            IEnumerable<PeriodFlowSettingModel> flowSettings,
            PeriodEmployeeSettingModel employeeSetting)
        {
            if (flowSettings != null)
            {
                var setting = employeeSetting;

                IEnumerable<PeriodFlowSettingModel> employeeFlowSettings =
                    flowSettings.Where(x => x.PeriodId == setting.PeriodId && x.EmployeeId == setting.Employee.EmployeeId);

                foreach (PeriodFlowSettingModel employeeFlowSetting in employeeFlowSettings)
                {
                    GetEvaluators(employeeFlowSetting, employeeSetting);
                }
            }
        }

        private static void GetEvaluators(PeriodFlowSettingModel flowSetting, PeriodEmployeeSettingModel employeeSetting)
        {
            PeriodFlowModel periodFlowModel = CreateEvaluator(flowSetting);
            switch (flowSetting.ProcessId)
            {
                case (int)ProcessType.HumanResource:
                    employeeSetting.HumanResources.Add(periodFlowModel);
                    break;

                case (int)ProcessType.Self:
                    employeeSetting.SelfEvaluators.Add(periodFlowModel);
                    break;

                case (int)ProcessType.Evaluator:
                    employeeSetting.Evaluators.Add(periodFlowModel);
                    break;

                case (int)ProcessType.Reviewer:
                    employeeSetting.Reviewers.Add(periodFlowModel);
                    break;

                case (int)ProcessType.Management:
                    employeeSetting.Managements.Add(periodFlowModel);
                    break;

                default:
                    return;
            }
        }

        private static PeriodFlowModel CreateEvaluator(PeriodFlowSettingModel rawFlowSetting)
        {
            var evaluator = new PeriodFlowModel
            {
                ProcessId = rawFlowSetting.ProcessId,
                Priority = rawFlowSetting.Priority,
                Duration = rawFlowSetting.Duration,
                RemindDay = rawFlowSetting.RemindDay,
                StartDate = rawFlowSetting.StartDate,
                EndDate = rawFlowSetting.EndDate,
                HasSendMail = rawFlowSetting.HasSendMail,
                HasEvaluationGenerated = rawFlowSetting.HasEvaluationGenerated,
                ProcessStatus = rawFlowSetting.ProcessStatus,
                ProcessDetailStatus = rawFlowSetting.ProcessDetailStatus,
                EvaluatorInfo = rawFlowSetting.EvaluatorInfo
            };

            return evaluator;
        }
    }
}