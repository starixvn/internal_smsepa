// ----------------------------------------------------------------------
// <copyright file="AutoMailService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/03/2015">Created.</change>
// </history>

namespace SMS.EPA.AutoGenerateEvaluation.Service.Implement
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using log4net;
    using SMS.EPA.AutoGenerateEvaluation.Service.Interfaces;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class AutoMailService : IAutoMailService
    {
        private string _startDateMailToHumanResources = "Views\\Emails\\StartDateEmailToHR.cshtml";
        private string _startDateMailToSelf = "Views\\Emails\\StartDateEmailToSelf.cshtml";
        private string _startDateMailToEvaluator = "Views\\Emails\\StartDateEmailToEvaluator.cshtml";
        private string _startDateMailToSpecialEvaluator = "Views\\Emails\\StartDateEmailToSpecialEvaluator.cshtml";
        private string _startDateMailToReviewer = "Views\\Emails\\StartDateEmailToReviewer.cshtml";
        private string _startDateMailToSpecialReviewer = "Views\\Emails\\StartDateEmailToSpecialReviewer.cshtml";
        private string _startDateMailToManagement = "Views\\Emails\\StartDateEmailToManagement.cshtml";
        private string _startDateMailToSpecialManagement = "Views\\Emails\\StartDateEmailToSpecialManagement.cshtml";

        private string _reminderMailToSelf = "Views\\Emails\\RemindDateEmailToSelf.cshtml";
        private string _reminderMailToEvaluator = "\\Views\\Emails\\RemindDateEmailToEvaluator.cshtml";
        private string _reminderMailToSpecialEvaluator = "Views\\Emails\\RemindDateEmailToSpecialEvaluator.cshtml";
        private string _reminderMailToReviewer = "Views\\Emails\\RemindDateEmailToReviewer.cshtml";
        private string _reminderMailToSpecialReviewer = "Views\\Emails\\RemindDateEmailToSpecialReviewer.cshtml";
        private string _reminderMailToManagement = "Views\\Emails\\RemindDateEmailToManagement.cshtml";
        private string _reminderMailToSpecialManagement = "Views\\Emails\\RemindDateEmailToSpecialManagement.cshtml";

        private IMailService _mailService;

        public AutoMailService()
            : this(null)
        {
        }

        public AutoMailService(IMailService mailService)
        {
            _mailService = mailService ?? new MailService();
        }

        public void SendStartMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings)
        {
            string templateEmailPath = string.Empty;
            string assemblyPath = AppDomain.CurrentDomain.BaseDirectory;

            foreach (PeriodFlowModel employeeSetting in employeeSettings)
            {
                switch (employeeSetting.ProcessId)
                {
                    case (int)ProcessType.HumanResource:
                        templateEmailPath = Path.Combine(assemblyPath, _startDateMailToHumanResources);
                        _mailService.SendMailToHRStartDate(templateEmailPath, employeeSetting, period.Name);
                        break;

                    case (int)ProcessType.Self:
                        templateEmailPath = Path.Combine(assemblyPath, _startDateMailToSelf);
                        _mailService.SendMailToSelfStartDate(templateEmailPath, employeeSetting, period.Name);
                        break;

                    case (int)ProcessType.Evaluator:
                        if (employeeSetting.EvaluatorInfo.IsSpecialUser)
                        {
                            templateEmailPath = Path.Combine(assemblyPath, _startDateMailToSpecialEvaluator);
                        }
                        else
                        {
                            templateEmailPath = Path.Combine(assemblyPath, _startDateMailToEvaluator);
                        }

                        _mailService.SendMailToEvaluatorStartDate(templateEmailPath, employeeSetting, period.Name);
                        break;

                    case (int)ProcessType.Reviewer:
                        if (employeeSetting.EvaluatorInfo.IsSpecialUser)
                        {
                            templateEmailPath = Path.Combine(assemblyPath, _startDateMailToSpecialReviewer);
                        }
                        else
                        {
                            templateEmailPath = Path.Combine(assemblyPath, _startDateMailToReviewer);
                        }

                        _mailService.SendMailToReviewerStartDate(templateEmailPath, employeeSetting, period.Name);
                        break;

                    case (int)ProcessType.Management:
                        if (employeeSetting.EvaluatorInfo.IsSpecialUser)
                        {
                            templateEmailPath = Path.Combine(assemblyPath, _startDateMailToSpecialManagement);
                        }
                        else
                        {
                            templateEmailPath = Path.Combine(assemblyPath, _startDateMailToManagement);
                        }

                        _mailService.SendMailToManagementStartDate(templateEmailPath, employeeSetting, period.Name);
                        break;
                }
            }
        }

        public void SendReminderMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings)
        {
            string templateEmailPath = string.Empty;
            string assemblyPath = AppDomain.CurrentDomain.BaseDirectory;

            foreach (PeriodFlowModel employeeSetting in employeeSettings)
            {
                switch (employeeSetting.ProcessId)
                {
                    case (int)ProcessType.Self:
                        templateEmailPath = Path.Combine(assemblyPath, _reminderMailToSelf);
                        _mailService.SendMailToSelfRemindDate(templateEmailPath, employeeSetting, period.Name);
                        break;

                    case (int)ProcessType.Evaluator:
                        if (employeeSetting.EvaluatorInfo.IsSpecialUser)
                        {
                            templateEmailPath = Path.Combine(assemblyPath, _reminderMailToSpecialEvaluator);
                        }
                        else
                        {
                            templateEmailPath = Path.Combine(assemblyPath, _reminderMailToEvaluator);
                        }

                        _mailService.SendMailToEvaluatorRemindDate(templateEmailPath, employeeSetting, period.Name);
                        break;

                    case (int)ProcessType.Reviewer:
                        if (employeeSetting.EvaluatorInfo.IsSpecialUser)
                        {
                            templateEmailPath = Path.Combine(assemblyPath, _reminderMailToSpecialReviewer);
                        }
                        else
                        {
                            templateEmailPath = Path.Combine(assemblyPath, _reminderMailToReviewer);
                        }

                        _mailService.SendMailToReviewerRemindDate(templateEmailPath, employeeSetting, period.Name);
                        break;

                    case (int)ProcessType.Management:
                        if (employeeSetting.EvaluatorInfo.IsSpecialUser)
                        {
                            templateEmailPath = Path.Combine(assemblyPath, _reminderMailToSpecialManagement);
                        }
                        else
                        {
                            templateEmailPath = Path.Combine(assemblyPath, _reminderMailToManagement);
                        }

                        _mailService.SendMailToManagementRemindDate(templateEmailPath, employeeSetting, period.Name);
                        break;
                }
            }
        }
    }
}
