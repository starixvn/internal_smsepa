// ----------------------------------------------------------------------
// <copyright file="PeriodService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/13/2015">Created.</change>
// </history>

namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using Fanex.Common.Helper;
    using NexFB.Core.Utilities;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.Period;
    using SMS.EPA.Criteria.Period.GenerationEvaluation;
    using SMS.EPA.Model.FlowManagement;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Repositories.Implement;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Resources;
    using SMS.EPA.Services.Interfaces;

    public class PeriodService : BaseService, IPeriodService
    {
        private readonly IPeriodRepository _periodRepository;
        private readonly IManualMailService _manualMailService;

        public PeriodService()
            : this(null, null)
        {
        }

        public PeriodService(IPeriodRepository periodRepository, IManualMailService manualMailService)
        {
            _periodRepository = periodRepository ?? new PeriodRepository();
            _manualMailService = manualMailService ?? new ManualMailService();
        }

        public IEnumerable<PeriodModel> GetPeriod(GetPeriodCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<PeriodModel> periods = new List<PeriodModel>();

            if (criteria.IsValid())
            {
                periods = _periodRepository.GetPeriod(criteria);
            }

            return periods;
        }

        public CommandResult CreatePeriod(CreatePeriodCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateBasicInformationPeriod(UpdateBasicInformationPeriodCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public PeriodModel GetPeriodById(GetPeriodByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            PeriodModel period = new PeriodModel();

            if (criteria.IsValid())
            {
                period = _periodRepository.GetPeriodById(criteria);
            }

            return period;
        }

        public CommandResult UpdateStatusPeriod(UpdateStatusPeriodCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult DeletePeriod(DeletePeriodCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Delete(criteria);
            }

            return commandResult;
        }

        public IEnumerable<PeriodEmployeeSettingModel> GetPeriodEmployeeSettingById(GetPeriodEmployeeSettingCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<PeriodEmployeeSettingModel> periodEmployeeSettings = new List<PeriodEmployeeSettingModel>();

            if (criteria.IsValid())
            {
                RawPeriodEmployeeSettingModel rawPeriodEmployeeSetting = _periodRepository.GetPeriodEmployeeSettingById(criteria);

                if (rawPeriodEmployeeSetting.Employees != null && rawPeriodEmployeeSetting.Employees.Count() > 0)
                {
                    periodEmployeeSettings = ConvertToPeriodEmployeeSetting(rawPeriodEmployeeSetting);
                }
            }

            return periodEmployeeSettings;
        }

        public CommandResult AddEmployeeToPeriod(AddEmployeeToPeriodCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateEmployeeSettingOfPeriod(UpdateEmployeeSettingOfPeriodCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);

                if (commandResult.IsSuccessful && ConfigurationManager.AppSettings["SendMailWhenEditEmployeeSetting"].Equals("true"))
                {
                    GetAndSendEmailToAllRelatedEmployee(MailMode.NotifyChange, criteria.PeriodId, criteria.EmployeeIds, null);
                }
            }

            return commandResult;
        }

        public CommandResult UpdateIncludeStatusEmployeePeriod(UpdateIncludeStatusEmployeePeriodCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult GenerateEvaluation(AutoGenerateEvaluationCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            var commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        public void SendEmailNoticeAtBegin(int? periodId)
        {
            var criteria = new GetPeriodHasStartedProcessCriteria
            {
                PeriodId = periodId
            };
            IEnumerable<AutoEmployeePeriodModel> startedProcess = GetPeriodHasStartedProcess(criteria);

            foreach (AutoEmployeePeriodModel periodProcess in startedProcess)
            {
                GetAndSendEmailToAllRelatedEmployee(MailMode.Start, periodProcess.PeriodId, string.Empty, periodProcess.ProcessId);
            }
        }

        public IEnumerable<PeriodFlowModel> GetEmployeeSettingByPeriodId(GetEmployeeSettingByPeriodIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<PeriodFlowModel> employeeSettings = new List<PeriodFlowModel>();

            if (criteria.IsValid())
            {
                employeeSettings = _periodRepository.GetEmployeeSettingByPeriodId(criteria);
            }

            return employeeSettings;
        }

        public IEnumerable<AutoEmployeePeriodModel> GetPeriodHasStartedProcess(GetPeriodHasStartedProcessCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<AutoEmployeePeriodModel> startedPeriods = new List<AutoEmployeePeriodModel>();

            if (criteria.IsValid())
            {
                startedPeriods = GetData<AutoEmployeePeriodModel>(criteria);
            }

            return startedPeriods;
        }

        public IEnumerable<FlowModel> GetProcessSettingByPeriodId(GetPeriodProcessSettingCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<FlowModel> processSettings = new List<FlowModel>();

            if (criteria.IsValid())
            {
                processSettings = GetData<FlowModel>(criteria);
            }

            return processSettings;
        }

        public CommandResult UpdatePeriodProcessSetting(UpdatePeriodProcessSettingCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public bool CheckSelfProcessStarted(CheckSelfProcessStartedCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            bool hasStarted = false;

            if (criteria.IsValid())
            {
                hasStarted = GetData<bool>(criteria).First();
            }

            return hasStarted;
        }

        public void ExportEmployeeSettingReport(
            IEnumerable<PeriodEmployeeSettingModel> employeeFlowSettings,
            string templatePath,
            string periodName)
        {
            string reportName = string.Format(Lang.EmployeeFlowSettingReportName, periodName);
            reportName = reportName.Replace(" ", "_");
            string reportTitle = string.Format(Lang.EmployeeFlowSettingReportTitle, periodName);
            var writer = new ExcelTemplateWriter(templatePath, reportName, Lang.EmployeeFlowSettingReportSheet);

            // Fill report title.
            writer.WriteRow(0, new object[] { reportTitle });
            writer.CopyRows(1, 1);

            // Fill report header.
            writer.CopyRows(3, 1);

            FillContentRow(employeeFlowSettings, ref writer);
            writer.SyncColumnsWidth(10);

            writer.Render();
        }

        private static IEnumerable<PeriodEmployeeSettingModel> ConvertToPeriodEmployeeSetting(RawPeriodEmployeeSettingModel rawPeriodEmployeeSetting)
        {
            IEnumerable<PeriodEmployeeSettingModel> employeeSettings = rawPeriodEmployeeSetting.Employees;

            foreach (PeriodEmployeeSettingModel employeeSetting in employeeSettings)
            {
                GetFlowSetting(rawPeriodEmployeeSetting.FlowSettings, employeeSetting);
            }

            return employeeSettings;
        }

        private static void GetFlowSetting(
            IEnumerable<RawPeriodFlowSettingModel> rawFlowSettings,
            PeriodEmployeeSettingModel employeeSetting)
        {
            if (rawFlowSettings != null)
            {
                IEnumerable<RawPeriodFlowSettingModel> rawEmployeeFlowSettings =
                           rawFlowSettings.Where(x => x.PeriodId == employeeSetting.PeriodId && x.EmployeeId == employeeSetting.Employee.EmployeeId);

                foreach (RawPeriodFlowSettingModel rawFlowSetting in rawEmployeeFlowSettings)
                {
                    employeeSetting = GetEvaluators(rawFlowSetting, employeeSetting);

                    if (rawFlowSetting.ProcessId != (int)ProcessType.HumanResource)
                    {
                        employeeSetting.NumberEvaluators++;
                    }
                }
            }
        }

        private static PeriodEmployeeSettingModel GetEvaluators(
            RawPeriodFlowSettingModel rawFlowSetting,
            PeriodEmployeeSettingModel employeeSetting)
        {
            switch (rawFlowSetting.ProcessId)
            {
                case (int)ProcessType.HumanResource:
                    PeriodFlowModel humanResources = CreateEvaluator(rawFlowSetting);
                    employeeSetting.HumanResources.Add(humanResources);
                    break;

                case (int)ProcessType.Self:
                    PeriodFlowModel selfEvaluator = CreateEvaluator(rawFlowSetting);
                    employeeSetting.SelfEvaluators.Add(selfEvaluator);
                    break;

                case (int)ProcessType.Evaluator:
                    PeriodFlowModel evaluator = CreateEvaluator(rawFlowSetting);
                    employeeSetting.Evaluators.Add(evaluator);
                    break;

                case (int)ProcessType.Reviewer:
                    PeriodFlowModel reviewer = CreateEvaluator(rawFlowSetting);
                    employeeSetting.Reviewers.Add(reviewer);
                    break;

                case (int)ProcessType.Management:
                    PeriodFlowModel management = CreateEvaluator(rawFlowSetting);
                    employeeSetting.Managements.Add(management);
                    break;
            }

            return employeeSetting;
        }

        private static PeriodFlowModel CreateEvaluator(RawPeriodFlowSettingModel rawFlowSetting)
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
                EvaluatorInfo = rawFlowSetting.EvaluatorInfo
            };

            return evaluator;
        }

        private void FillContentRow(
         IEnumerable<PeriodEmployeeSettingModel> employeeFlowSettings,
         ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (PeriodEmployeeSettingModel employeeSetting in employeeFlowSettings)
            {
                FillRow(employeeSetting, ref writer, ref rowIndex);
            }
        }

        private void FillRow(
            PeriodEmployeeSettingModel employeeSetting,
            ref ExcelTemplateWriter writer,
            ref int rowIndex)
        {
            object record = CreateRecord(employeeSetting, ref rowIndex);
            writer.WriteRow(4, record);
        }

        private object CreateRecord(
            PeriodEmployeeSettingModel employeeSetting,
            ref int rowIndex)
        {
            var record = new
            {
                Count = rowIndex,
                Department = employeeSetting.Department != null ? employeeSetting.Department.Name : string.Empty,
                Section = employeeSetting.Group != null ? employeeSetting.Group.GroupName : string.Empty,
                ID = employeeSetting.Employee.Code,
                FullName = employeeSetting.Employee.FullName,
                NickName = employeeSetting.Employee.NickName,
                Evaluator = GetEvaluator(employeeSetting.Evaluators),
                Reviewer = GetEvaluator(employeeSetting.Reviewers),
                Management = GetEvaluator(employeeSetting.Managements),
                Form = employeeSetting.TargetTemplate != null ? employeeSetting.TargetTemplate.TargetTypeName : string.Empty
            };
            rowIndex++;

            return record;
        }

        private object GetEvaluator(IList<PeriodFlowModel> evaluators)
        {
            string evaluatorNames = string.Empty;

            foreach (PeriodFlowModel evaluator in evaluators)
            {
                string name = evaluator.EvaluatorInfo.NickName;
                evaluatorNames += !string.IsNullOrEmpty(evaluatorNames) ? ", " + name : name;
            }

            return evaluatorNames;
        }

        private void GetAndSendEmailToAllRelatedEmployee(MailMode mailMode, int periodId, string employeeIds, int? processId)
        {
            PeriodModel period = GetPeriodInformation(periodId);
            IEnumerable<PeriodFlowModel> employeeSettings = GetEmployeeSettings(periodId, employeeIds, processId);

            switch (mailMode)
            {
                case MailMode.Start:
                    _manualMailService.SendStartMail(period, employeeSettings);
                    break;
                case MailMode.NotifyChange:
                    _manualMailService.SendNotifyMail(period, employeeSettings);
                    break;
            }
        }

        private PeriodModel GetPeriodInformation(int periodId)
        {
            var period = new PeriodModel();

            var criteria = new GetPeriodByIdCriteria
            {
                PeriodId = periodId
            };
            period = GetPeriodById(criteria);

            return period;
        }

        private IEnumerable<PeriodFlowModel> GetEmployeeSettings(int periodId, string employeeIds, int? processId)
        {
            IEnumerable<PeriodFlowModel> employeeSettings = new List<PeriodFlowModel>();

            var getEmployeeSettingCriteria = new GetEmployeeSettingByPeriodIdCriteria
            {
                PeriodId = periodId,
                EmployeeIds = employeeIds,
                ProcessId = processId
            };
            employeeSettings = GetEmployeeSettingByPeriodId(getEmployeeSettingCriteria);

            return employeeSettings;
        }
    }
}
