namespace SMS.EPA.Services.UnitTests
{
    using System.Collections.Generic;
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Model;
    using SMS.EPA.Model.AutoService;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    [TestFixture]
    public class ManualMailServiceTests
    {
        private IMailService mailService;
        private IPeriodMailService manualMailService;

        /*[Test]
        public void SendStartMail_HRProcess_ExpectedToSendMailToHR()
        {
            // Arrange
            var employeeSettings = new List<PeriodFlowModel>();
            employeeSettings.Add(new PeriodFlowModel()
            {
                ProcessId = (byte)ProcessType.HumanResource
            });
            ArrangeEmail();

            // Act
            manualMailService.SendStartMail(new PeriodModel(), employeeSettings);

            // Assert
            mailService.Received().SendMailToHrStartDate(Arg.Any<string>(), Arg.Any<PeriodFlowModel>(), Arg.Any<string>());
        }

        [Test]
        public void SendStartMail_SelfProcess_ExpectedToSendMailToSelfEvaluation()
        {
            // Arrange
            var employeeSettings = new List<PeriodFlowModel>();
            employeeSettings.Add(new PeriodFlowModel()
            {
                ProcessId = (byte)ProcessType.Self
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
                ProcessId = (byte)ProcessType.Evaluator
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
                ProcessId = (byte)ProcessType.Reviewer
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
                ProcessId = (byte)ProcessType.Management
            });
            ArrangeEmail();

            // Act
            manualMailService.SendStartMail(new PeriodModel(), employeeSettings);

            // Assert
            mailService.Received().SendMailToManagementStartDate(Arg.Any<string>(), Arg.Any<PeriodFlowModel>(), Arg.Any<string>());
        }
        */

        [Test]
        public void SendNotifyMail_ValidArg_ExpectedToSendMailToNotifyChangeSetting()
        {
            // Arrange
            var employeeSettings = new List<EmployeeSettingToSendMail>
            {
                new EmployeeSettingToSendMail
                {
                    ProcessId = (byte)ProcessType.Management
                }
            };
            ArrangeEmail();

            // Act
            manualMailService.SendNotifyMail(employeeSettings);

            // Assert
            mailService.Received().SendMailToNotifyChangeSetting(Arg.Any<string>(), Arg.Any<EmployeeSettingToSendMail>());
        }

        private void ArrangeEmail()
        {
            mailService = Substitute.For<IMailService>();
            manualMailService = new PeriodMailService(mailService);
        }
    }
}