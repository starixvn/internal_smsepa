namespace SMS.EPA.Services.UnitTests
{
    using Implement;
    using Interfaces;
    using Model;
    using Model.Email;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class MailServiceTests
    {
        private IBaseMailService baseMailService;
        private IMailService mailService;

        [Test]
        public void SendMailReminder_ValidArg_ExpectedToSendMail()
        {
            // Arrange
            ArrangeEmail();

            // Act
            mailService.SendMailReminder("template", new PeriodFlowModel(), Arg.Any<string>(), Arg.Any<int>());

            // Assert
            baseMailService.Received().SendEmail(Arg.Any<MailModel>());
        }

        [Test]
        public void SendMailToNotifyChangeSetting_ValidArg_ExpectedToSendMail()
        {
            // Arrange
            ArrangeEmail();

            // Act
            mailService.SendMailToNotifyChangeSetting("template", new PeriodFlowModel(), Arg.Any<string>());

            // Assert
            baseMailService.Received().SendEmail(Arg.Any<MailModel>());
        }

        [Test]
        public void SendMailToSelfEndEvaluation_HavingReviewerAndManagementTotalScore_ExpectedToSendMail()
        {
            // Arrange
            var epaModel = new EPAModel();
            epaModel.Process.Managements.Add(new EPAProcessModel { TotalScore = "30" });
            epaModel.Process.Reviewers.Add(new EPAProcessModel { TotalScore = "30" });

            ArrangeEmail();

            // Act
            mailService.SendMailToSelfEndEvaluation("template", epaModel, Arg.Any<float>(), Arg.Any<float>());

            // Assert
            baseMailService.Received().SendEmail(Arg.Any<MailModel>());
        }

        [Test]
        public void SendMailToSelfEndEvaluation_HavingManagementTotalScore_ExpectedToSendMail()
        {
            // Arrange
            var epaModel = new EPAModel();
            epaModel.Process.Managements.Add(new EPAProcessModel { TotalScore = "30" });

            ArrangeEmail();

            // Act
            mailService.SendMailToSelfEndEvaluation("template", epaModel, Arg.Any<float>(), Arg.Any<float>());

            // Assert
            baseMailService.Received().SendEmail(Arg.Any<MailModel>());
        }

        [Test]
        public void SendMailToSelfEndEvaluation_HavingRevieweTotalScore_ExpectedToSendMail()
        {
            // Arrange
            var epaModel = new EPAModel();
            epaModel.Process.Reviewers.Add(new EPAProcessModel { TotalScore = "30" });

            ArrangeEmail();

            // Act
            mailService.SendMailToSelfEndEvaluation("template", epaModel, Arg.Any<float>(), Arg.Any<float>());

            // Assert
            baseMailService.Received().SendEmail(Arg.Any<MailModel>());
        }

        [Test]
        public void SendMailEndToHumanResourcesByEmployee_ValidArg_ExpectedToSendMail()
        {
            // Arrange
            ArrangeEmail();

            // Act
            mailService.SendMailEndToHumanResourcesByEmployee("template", new EPAModel(), new EPAProcessModel());

            // Assert
            baseMailService.Received().SendEmail(Arg.Any<MailModel>());
        }

        [Test]
        public void SendMailEndToHumanResourceByPeriod_ValidArg_ExpectedToSendMail()
        {
            // Arrange
            ArrangeEmail();

            // Act
            mailService.SendMailEndToHumanResourceByPeriod("template", new PeriodFlowModel(), Arg.Any<string>());

            // Assert
            baseMailService.Received().SendEmail(Arg.Any<MailModel>());
        }

        private void ArrangeEmail()
        {
            baseMailService = Substitute.For<IBaseMailService>();
            mailService = new MailService(baseMailService);
        }
    }
}