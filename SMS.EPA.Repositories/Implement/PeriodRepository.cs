namespace SMS.EPA.Repositories.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Repositories.Interfaces;

    public class PeriodRepository : IPeriodRepository
    {
        public IEnumerable<PeriodModel> GetPeriod(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<PeriodModel, TargetType, PeriodModel>(
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
                return db.QueryEntity<PeriodModel, TargetType, PeriodModel>(
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
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IEnumerable<PeriodEmployeeSettingModel> employeeSettings =
                        multi.Read<PeriodEmployeeSettingModel, Employee, Department, Group, JobTitle, TargetTemplate, PeriodEmployeeSettingModel>(
                            (pe, emp, dep, gr, job, tar) =>
                            {
                                pe.Employee = emp;
                                pe.Department = dep;
                                pe.Group = gr;
                                pe.JobTitle = job;
                                pe.TargetTemplate = tar;
                                return pe;
                            },
                            "EmployeeId, DepartmentId, GroupId, JobTitleId, TargetTemplateId");

                    IEnumerable<PeriodFlowSettingModel> flowSettings =
                        multi.Read<PeriodFlowSettingModel, EvaluatorInfoViewModel, PeriodFlowSettingModel>(
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

        public IEnumerable<PeriodFlowModel> GetEmployeeSettingByPeriodId(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                return db.Query<PeriodFlowModel, EvaluatorInfoViewModel, PeriodFlowModel>(
                    (empS, eva) =>
                    {
                        empS.EvaluatorInfo = eva;
                        return empS;
                    },
                    "EvaluatorId",
                    criteria.GetSpParams());
            }
        }

        private static void GetFlowSetting(
            IEnumerable<PeriodFlowSettingModel> flowSettings,
            PeriodEmployeeSettingModel employeeSetting)
        {
            if (flowSettings != null)
            {
                IEnumerable<PeriodFlowSettingModel> employeeFlowSettings =
                           flowSettings.Where(x => x.PeriodId == employeeSetting.PeriodId && x.EmployeeId == employeeSetting.Employee.EmployeeId);

                foreach (PeriodFlowSettingModel employeeFlowSetting in employeeFlowSettings)
                {
                    employeeSetting = GetEvaluators(employeeFlowSetting, employeeSetting);

                    if (employeeFlowSetting.ProcessId != (int)ProcessType.HumanResource)
                    {
                        employeeSetting.NumberEvaluators++;
                    }
                }
            }
        }

        private static PeriodEmployeeSettingModel GetEvaluators(
            PeriodFlowSettingModel flowSetting,
            PeriodEmployeeSettingModel employeeSetting)
        {
            switch (flowSetting.ProcessId)
            {
                case (int)ProcessType.HumanResource:
                    PeriodFlowModel humanResources = CreateEvaluator(flowSetting);
                    employeeSetting.HumanResources.Add(humanResources);
                    break;

                case (int)ProcessType.Self:
                    PeriodFlowModel selfEvaluator = CreateEvaluator(flowSetting);
                    employeeSetting.SelfEvaluators.Add(selfEvaluator);
                    break;

                case (int)ProcessType.Evaluator:
                    PeriodFlowModel evaluator = CreateEvaluator(flowSetting);
                    employeeSetting.Evaluators.Add(evaluator);
                    break;

                case (int)ProcessType.Reviewer:
                    PeriodFlowModel reviewer = CreateEvaluator(flowSetting);
                    employeeSetting.Reviewers.Add(reviewer);
                    break;

                case (int)ProcessType.Management:
                    PeriodFlowModel management = CreateEvaluator(flowSetting);
                    employeeSetting.Managements.Add(management);
                    break;
            }

            return employeeSetting;
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