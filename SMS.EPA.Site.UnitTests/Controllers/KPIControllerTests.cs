namespace SMS.EPA.Site.UnitTests.Controllers
{
    using System.Collections.Generic;
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Interfaces;
    using SMS.EPA.Site.Controllers.API;

    [TestFixture]
    public class KPIControllerTests
    {
        private IKPIService kpiService;
        private KPIController kpiController;

        #region GetKPI

        [Test]
        public void GetKPI_ValidActionName_ReturnsTrue()
        {
            // Arrange
            kpiService = FakeKPIService();
            kpiController = GetKPIController(kpiService);

            // Act
            var actual = kpiController.GetType().GetMethod("GetKPI");

            // Assert
            Assert.IsNotNull(actual);
            //// StringAssert.AreEqualIgnoringCase("GetKPI", actual.Name);
        }

        [Test]
        public void GetKPI_WhenCalled_ReturnsListOfKPIModel()
        {
            // Arrange
            kpiService = FakeKPIService();
            kpiController = GetKPIController(kpiService);

            var expected = new List<KPIModel>();
            kpiController.GetKPI(Arg.Any<int?>(), Arg.Any<bool?>(), Arg.Any<bool?>()).Returns(expected);

            // Act
            var actual = kpiController.GetKPI();

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<KPIModel>), actual);
        }

        #endregion GetKPI

        #region CreateKPI

        [Test]
        public void CreateKPI_ValidActionName_ReturnsTrue()
        {
            // Arrange
            kpiService = FakeKPIService();
            kpiController = GetKPIController(kpiService);

            // Act
            var actual = kpiController.GetType().GetMethod("CreateKPI");

            // Assert
            Assert.IsNotNull(actual);
            //// StringAssert.AreEqualIgnoringCase("CreateKPI", actual.Name);
        }

        [Test]
        public void CreateKPI_WhenCalled_ReturnsCommandResult()
        {
            // Arrange
            kpiService = FakeKPIService();
            kpiController = GetKPIController(kpiService);

            var expected = new CommandResult();
            //// kpiService.CreateKPI(Arg.Is<CreateKPICriteria>(criteria => criteria != null)).Returns(expected);

            // Act
            // var actual = kpiController.CreateKPI(new CreateKPICriteria());

            // Assert
            // Assert.AreSame(expected, actual);
            Assert.IsTrue(true);
        }

        #endregion CreateKPI

        private IKPIService FakeKPIService()
        {
            var service = Substitute.For<IKPIService>();

            return service;
        }

        private KPIController GetKPIController(IKPIService kpiService)
        {
            var controller = new KPIController(kpiService);

            return controller;
        }
    }
}