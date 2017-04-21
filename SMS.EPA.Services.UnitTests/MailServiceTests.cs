namespace SMS.EPA.Services.UnitTests
{
    using System;
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Core;
    using SMS.EPA.Core.Services.Interfaces;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    [TestFixture]
    public class MailServiceTests
    {
        private IBaseMailService baseMailService;
        private IMailService mailService;

        [Test]
        public void SendMailToHRStartDate_ValidArg_ExpectedToSendMail()
        {
            // Arrange
            var periodFlow = new PeriodFlowModel()
            {
                StartDate = new DateTime(),
                EndDate = new DateTime()
            };

            ArrangeEmail();

            // Act
            mailService.SendMailToHrStartDate(Arg.Any<string>(), periodFlow, Arg.Any<string>());

            // Assert
            baseMailService.Received().SendEmail(Arg.Any<MailModel>());
        }

        [Test]
        public void SendMailToSelfStartDate_ValidArg_ExpectedToSendMail()
        {
            // Arrange
            ArrangeEmail();

            // Act
            mailService.SendMailToSelfStartDate(Arg.Any<string>(), new PeriodFlowModel(), Arg.Any<string>());

            // Assert
            baseMailService.Received().SendEmail(Arg.Any<MailModel>());
        }

        [Test]
        public void SendMailToEvaluatorStartDate_ValidArg_ExpectedToSendMail()
        {
            // Arrange
            ArrangeEmail();

            // Act
            mailService.SendMailToEvaluatorStartDate(Arg.Any<string>(), new PeriodFlowModel(), Arg.Any<string>());

            // Assert
            baseMailService.Received().SendEmail(Arg.Any<MailModel>());
        }

        [Test]
        public void SendMailToManagementStartDate_ValidArg_ExpectedToSendMail()
        {
            // Arrange
            ArrangeEmail();

            // Act
            mailService.SendMailToManagementStartDate(Arg.Any<string>(), new PeriodFlowModel(), Arg.Any<string>());

            // Assert
            baseMailService.Received().SendEmail(Arg.Any<MailModel>());
        }

        [Test]
        public void SendMailToReviewerStartDate_ValidArg_ExpectedToSendMail()
        {
            // Arrange
            ArrangeEmail();

            // Act
            mailService.SendMailToReviewerStartDate(Arg.Any<string>(), new PeriodFlowModel(), Arg.Any<string>());

            // Assert
            baseMailService.Received().SendEmail(Arg.Any<MailModel>());
        }

        [Test]
        public void SendMailToSelfRemindDate_ValidArg_ExpectedToSendMail()
        {
            // Arrange
            ArrangeEmail();

            // Act
            mailService.SendMailToSelfRemindDate(Arg.Any<string>(), new PeriodFlowModel(), Arg.Any<string>());

            // Assert
            baseMailService.Received().SendEmail(Arg.Any<MailModel>());
        }

        [Test]
        public void SendMailToEvaluatorRemindDate_ValidArg_ExpectedToSendMail()
        {
            // Arrange
            ArrangeEmail();

            // Act
            mailService.SendMailToEvaluatorRemindDate(Arg.Any<string>(), new PeriodFlowModel(), Arg.Any<string>());

            // Assert
            baseMailService.Received().SendEmail(Arg.Any<MailModel>());
        }

        [Test]
        public void SendMailToReviewerRemindDate_ValidArg_ExpectedToSendMail()
        {
            // Arrange
            ArrangeEmail();

            // Act
            mailService.SendMailToReviewerRemindDate(Arg.Any<string>(), new PeriodFlowModel(), Arg.Any<string>());

            // Assert
            baseMailService.Received().SendEmail(Arg.Any<MailModel>());
        }

        [Test]
        public void SendMailToManagementRemindDate_ValidArg_ExpectedToSendMail()
        {
            // Arrange
            ArrangeEmail();

            // Act
            mailService.SendMailToManagementRemindDate(Arg.Any<string>(), new PeriodFlowModel(), Arg.Any<string>());

            // Assert
            baseMailService.Received().SendEmail(Arg.Any<MailModel>());
        }

        [Test]
        public void SendMailToNotifyChangeSetting_ValidArg_ExpectedToSendMail()
        {
            // Arrange
            ArrangeEmail();

            // Act
            mailService.SendMailToNotifyChangeSetting(Arg.Any<string>(), new PeriodFlowModel(), Arg.Any<string>());

            // Assert
            baseMailService.Received().SendEmail(Arg.Any<MailModel>());
        }

        [Test]
        public void SendMailToSelfEndEvaluation_HavingReviewerAndManagementTotalScore_ExpectedToSendMail()
        {
            // Arrange
            var epaModel = new EPAModel();
            epaModel.Process.Managements.Add(new EPAProcessModel() { TotalScore = "30" });
            epaModel.Process.Reviewers.Add(new EPAProcessModel() { TotalScore = "30" });

            ArrangeEmail();

            // Act
            mailService.SendMailToSelfEndEvaluation(Arg.Any<string>(), epaModel, Arg.Any<float>(), Arg.Any<float>());

            // Assert
            baseMailService.Received().SendEmail(Arg.Any<MailModel>());
        }

        [Test]
        public void SendMailToSelfEndEvaluation_HavingManagementTotalScore_ExpectedToSendMail()
        {
            // Arrange
            var epaModel = new EPAModel();
            epaModel.Process.Managements.Add(new EPAProcessModel() { TotalScore = "30" });

            ArrangeEmail();

            // Act
            mailService.SendMailToSelfEndEvaluation(Arg.Any<string>(), epaModel, Arg.Any<float>(), Arg.Any<float>());

            // Assert
            baseMailService.Received().SendEmail(Arg.Any<MailModel>());
        }

        [Test]
        public void SendMailToSelfEndEvaluation_HavingRevieweTotalScore_ExpectedToSendMail()
        {
            // Arrange
            var epaModel = new EPAModel();
            epaModel.Process.Reviewers.Add(new EPAProcessModel() { TotalScore = "30" });

            ArrangeEmail();

            // Act
            mailService.SendMailToSelfEndEvaluation(Arg.Any<string>(), epaModel, Arg.Any<float>(), Arg.Any<float>());

            // Assert
            baseMailService.Received().SendEmail(Arg.Any<MailModel>());
        }

        [Test]
        public void SendMailEndToHumanResourcesByEmployee_ValidArg_ExpectedToSendMail()
        {
            // Arrange
            ArrangeEmail();

            // Act
            mailService.SendMailEndToHumanResourcesByEmployee(Arg.Any<string>(), new EPAModel(), new EPAProcessModel());

            // Assert
            baseMailService.Received().SendEmail(Arg.Any<MailModel>());
        }

        [Test]
        public void SendMailEndToHumanResourceByPeriod_ValidArg_ExpectedToSendMail()
        {
            // Arrange
            ArrangeEmail();

            // Act
            mailService.SendMailEndToHumanResourceByPeriod(Arg.Any<string>(), new PeriodFlowModel(), Arg.Any<string>());

            // Assert
            baseMailService.Received().SendEmail(Arg.Any<MailModel>());
        }

        ////[Test]
        ////public void ChangeStartDate_NotNull_ReturnExpected()
        ////{
        ////    // Arrange
        ////    var date = new DateTime(2016, 01, 01, 0, 0, 0);
        ////    ArrangeEmail();

        ////    // Act
        ////    var actual = mailService.ChangeStartDate(date);

        ////    // Assert
        ////    Assert.AreEqual("", actual);
        ////}

        private void ArrangeEmail()
        {
            baseMailService = Substitute.For<IBaseMailService>();
            mailService = new MailService(baseMailService);
        }
    }
}