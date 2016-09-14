namespace SMS.EPA.Services.UnitTests
{
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Core;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Services.Interfaces;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;
    using System;
    using System.Collections.Generic;

    [TestFixture]
    public class ManualMailServiceTests
    {
        private IMailService mailService;
        private IManualMailService manualMailService;

        [Test]
        public void SendStartMail_HRProcess_ExpectedToSendMailToHR()
        {
            // Arrange
            var employeeSettings = new List<PeriodFlowModel>();
            employeeSettings.Add(new PeriodFlowModel()
            {
                ProcessId = (int)ProcessType.HumanResource
            });
            ArrangeEmail();

            // Act
            manualMailService.SendStartMail(new PeriodModel(), employeeSettings);

            // Assert
            mailService.Received().SendMailToHRStartDate(Arg.Any<string>(), Arg.Any<PeriodFlowModel>(), Arg.Any<string>());
        }

        [Test]
        public void SendStartMail_SelfProcess_ExpectedToSendMailToSelfEvaluation()
        {
            // Arrange
            var employeeSettings = new List<PeriodFlowModel>();
            employeeSettings.Add(new PeriodFlowModel()
            {
                ProcessId = (int)ProcessType.Self
            });
            ArrangeEmail();

            // Act
            manualMailService.SendStartMail(new PeriodModel(), employeeSettings);

            // Assert
            mailService.Received().SendMailToSelfStartDate(Arg.Any<string>(), Arg.Any<PeriodFlowModel>(), Arg.Any<string>());
        }

        [Test]
        public void SendStartMail_EvaluatorProcess_ExpectedToSendMailToEvaluator()
        {
            // Arrange
            var employeeSettings = new List<PeriodFlowModel>();
            employeeSettings.Add(new PeriodFlowModel()
            {
                ProcessId = (int)ProcessType.Evaluator
            });
            ArrangeEmail();

            // Act
            manualMailService.SendStartMail(new PeriodModel(), employeeSettings);

            // Assert
            mailService.Received().SendMailToEvaluatorStartDate(Arg.Any<string>(), Arg.Any<PeriodFlowModel>(), Arg.Any<string>());
        }

        [Test]
        public void SendStartMail_ReviewerProcess_ExpectedToSendMailToReviewer()
        {
            // Arrange
            var employeeSettings = new List<PeriodFlowModel>();
            employeeSettings.Add(new PeriodFlowModel()
            {
                ProcessId = (int)ProcessType.Reviewer
            });
            ArrangeEmail();

            // Act
            manualMailService.SendStartMail(new PeriodModel(), employeeSettings);

            // Assert
            mailService.Received().SendMailToReviewerStartDate(Arg.Any<string>(), Arg.Any<PeriodFlowModel>(), Arg.Any<string>());
        }

        [Test]
        public void SendStartMail_ManagementProcess_ExpectedToSendMailToManagement()
        {
            // Arrange
            var employeeSettings = new List<PeriodFlowModel>();
            employeeSettings.Add(new PeriodFlowModel()
            {
                ProcessId = (int)ProcessType.Management
            });
            ArrangeEmail();

            // Act
            manualMailService.SendStartMail(new PeriodModel(), employeeSettings);

            // Assert
            mailService.Received().SendMailToManagementStartDate(Arg.Any<string>(), Arg.Any<PeriodFlowModel>(), Arg.Any<string>());
        }

        [Test]
        public void SendNotifyMail_ValidArg_ExpectedToSendMailToNotifyChangeSetting()
        {
            // Arrange
            var employeeSettings = new List<PeriodFlowModel>();
            employeeSettings.Add(new PeriodFlowModel()
            {
                ProcessId = (int)ProcessType.Management
            });
            ArrangeEmail();

            // Act
            manualMailService.SendNotifyMail(new PeriodModel(), employeeSettings);

            // Assert
            mailService.Received().SendMailToNotifyChangeSetting(Arg.Any<string>(), Arg.Any<PeriodFlowModel>(), Arg.Any<string>());
        }

        
        private void ArrangeEmail()
        {
            mailService = Substitute.For<IMailService>();
            manualMailService = new ManualMailService(mailService);
        }
    }
}
