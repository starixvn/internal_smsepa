namespace SMS.EPA.Repositories
{
    using Core.Enumerations;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;
    using Model.Enitites;
    using System.Collections.Generic;
    using System.Linq;

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
                using (GridReaderWrapper multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IEnumerable<PeriodEmployeeSettingModel> employeeSettings =
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

        private void GetFlowSetting(
            IEnumerable<PeriodFlowSettingModel> flowSettings,
            PeriodEmployeeSettingModel employeeSetting)
        {
            if (flowSettings != null)
            {
                var setting = employeeSetting;
                IEnumerable<PeriodFlowSettingModel> employeeFlowSettings =
                    flowSettings.Where(x => x.PeriodId == setting.PeriodId &&
                                            x.EmployeeId == setting.Employee.EmployeeId);

                foreach (PeriodFlowSettingModel employeeFlowSetting in employeeFlowSettings)
                {
                    employeeSetting = GetEvaluators(employeeFlowSetting, employeeSetting);
                }
            }
        }

        private PeriodEmployeeSettingModel GetEvaluators(PeriodFlowSettingModel flowSetting, PeriodEmployeeSettingModel employeeSetting)
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

        private PeriodFlowModel CreateEvaluator(PeriodFlowSettingModel rawFlowSetting)
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