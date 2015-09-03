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
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web;
    using SMS.EPA.AutoGenerateEvaluation.Service.Interfaces;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class AutoMailService : IAutoMailService
    {
        private string _startDateMailToHumanResources = "~/Views/Emails/StartDateEmailToHR.cshtml";
        private string _startDateMailToSelf = "~/Views/Emails/StartDateEmailToSelf.cshtml";
        private string _startDateMailToEvaluator = "~/Views/Emails/StartDateEmailToEvaluator.cshtml";
        private string _startDateMailToSpecialEvaluator = "~/Views/Emails/StartDateEmailToSpecialEvaluator.cshtml";
        private string _startDateMailToReviewer = "~/Views/Emails/StartDateEmailToReviewer.cshtml";
        private string _startDateMailToSpecialReviewer = "~/Views/Emails/StartDateEmailToSpecialReviewer.cshtml";
        private string _startDateMailToManagement = "~/Views/Emails/StartDateEmailToManagement.cshtml";
        private string _startDateMailToSpecialManagement = "~/Views/Emails/StartDateEmailToSpecialManagement.cshtml";

        private string _reminderMailToSelf = "~/Views/Emails/RemindDateEmailToSelf.cshtml";
        private string _reminderMailToEvaluator = "~/Views/Emails/RemindDateEmailToEvaluator.cshtml";
        private string _reminderMailToSpecialEvaluator = "~/Views/Emails/RemindDateEmailToSpecialEvaluator.cshtml";
        private string _reminderMailToReviewer = "~/Views/Emails/RemindDateEmailToReviewer.cshtml";
        private string _reminderMailToSpecialReviewer = "~/Views/Emails/RemindDateEmailToSpecialReviewer.cshtml";
        private string _reminderMailToManagement = "~/Views/Emails/RemindDateEmailToManagement.cshtml";
        private string _reminderMailToSpecialManagement = "~/Views/Emails/RemindDateEmailToSpecialManagement.cshtml";

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

            foreach (PeriodFlowModel employeeSetting in employeeSettings)
            {
                switch (employeeSetting.ProcessId)
                {
                    case (int)ProcessType.HumanResource:
                        templateEmailPath = HttpContext.Current.Server.MapPath(_startDateMailToHumanResources);
                        var taskHumanResourceSendMail = new Task(() => _mailService.SendMailToHRStartDate(templateEmailPath, employeeSetting, period.Name));
                        taskHumanResourceSendMail.Start();
                        break;

                    case (int)ProcessType.Self:
                        templateEmailPath = HttpContext.Current.Server.MapPath(_startDateMailToSelf);
                        var taskSelfSendMail = new Task(() => _mailService.SendMailToSelfStartDate(templateEmailPath, employeeSetting, period.Name));
                        taskSelfSendMail.Start();
                        break;

                    case (int)ProcessType.Evaluator:
                        if (employeeSetting.EvaluatorInfo.IsSpecialUser)
                        {
                            templateEmailPath = HttpContext.Current.Server.MapPath(_startDateMailToSpecialEvaluator);
                        }
                        else
                        {
                            templateEmailPath = HttpContext.Current.Server.MapPath(_startDateMailToEvaluator);
                        }

                        var taskEvaluatorSendMail = new Task(() => _mailService.SendMailToEvaluatorStartDate(templateEmailPath, employeeSetting, period.Name));
                        taskEvaluatorSendMail.Start();
                        break;

                    case (int)ProcessType.Reviewer:
                        if (employeeSetting.EvaluatorInfo.IsSpecialUser)
                        {
                            templateEmailPath = HttpContext.Current.Server.MapPath(_startDateMailToSpecialReviewer);
                        }
                        else
                        {
                            templateEmailPath = HttpContext.Current.Server.MapPath(_startDateMailToReviewer);
                        }

                        var taskReviewerSendMail = new Task(() => _mailService.SendMailToReviewerStartDate(templateEmailPath, employeeSetting, period.Name));
                        taskReviewerSendMail.Start();
                        break;

                    case (int)ProcessType.Management:
                        if (employeeSetting.EvaluatorInfo.IsSpecialUser)
                        {
                            templateEmailPath = HttpContext.Current.Server.MapPath(_startDateMailToSpecialManagement);
                        }
                        else
                        {
                            templateEmailPath = HttpContext.Current.Server.MapPath(_startDateMailToManagement);
                        }

                        var taskManagementSendMail = new Task(() => _mailService.SendMailToManagementStartDate(templateEmailPath, employeeSetting, period.Name));
                        taskManagementSendMail.Start();
                        break;
                }
            }
        }

        public void SendReminderMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings)
        {
            string templateEmailPath = string.Empty;

            foreach (PeriodFlowModel employeeSetting in employeeSettings)
            {
                switch (employeeSetting.ProcessId)
                {
                    case (int)ProcessType.Self:
                        templateEmailPath = HttpContext.Current.Server.MapPath(_reminderMailToSelf);
                        var taskSelfSendMail = new Task(() => _mailService.SendMailToSelfRemindDate(templateEmailPath, employeeSetting, period.Name));
                        taskSelfSendMail.Start();
                        break;

                    case (int)ProcessType.Evaluator:
                        if (employeeSetting.EvaluatorInfo.IsSpecialUser)
                        {
                            templateEmailPath = HttpContext.Current.Server.MapPath(_reminderMailToSpecialEvaluator);
                        }
                        else
                        {
                            templateEmailPath = HttpContext.Current.Server.MapPath(_reminderMailToEvaluator);
                        }

                        var taskEvaluatorSendMail = new Task(() => _mailService.SendMailToEvaluatorRemindDate(templateEmailPath, employeeSetting, period.Name));
                        taskEvaluatorSendMail.Start();
                        break;

                    case (int)ProcessType.Reviewer:
                        if (employeeSetting.EvaluatorInfo.IsSpecialUser)
                        {
                            templateEmailPath = HttpContext.Current.Server.MapPath(_reminderMailToSpecialReviewer);
                        }
                        else
                        {
                            templateEmailPath = HttpContext.Current.Server.MapPath(_reminderMailToReviewer);
                        }

                        var taskReviewerSendMail = new Task(() => _mailService.SendMailToReviewerRemindDate(templateEmailPath, employeeSetting, period.Name));
                        taskReviewerSendMail.Start();
                        break;

                    case (int)ProcessType.Management:
                        if (employeeSetting.EvaluatorInfo.IsSpecialUser)
                        {
                            templateEmailPath = HttpContext.Current.Server.MapPath(_reminderMailToSpecialManagement);
                        }
                        else
                        {
                            templateEmailPath = HttpContext.Current.Server.MapPath(_reminderMailToManagement);
                        }

                        var taskManagementSendMail = new Task(() => _mailService.SendMailToManagementRemindDate(templateEmailPath, employeeSetting, period.Name));
                        taskManagementSendMail.Start();
                        break;
                }
            }
        }
    }
}
