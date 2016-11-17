namespace SMS.EPA.Services.UnitTests
{
    using System.Collections.Generic;
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Criteria.TargetType;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Implement;

    [TestFixture]
    public class TargetTypeServiceTests
    {
        [Test]
        public void GetTargetType_AnyArg_ReturnExpectedTargetTypes()
        {
            // Arrange
            var expectedTargetTypes = new List<TargetTypeModel>();
            expectedTargetTypes.Add(new TargetTypeModel());
            var targetTypeService = ArrangeGetTargetType(expectedTargetTypes);

            // Act
            var actual = targetTypeService.GetTargetType(Arg.Any<GetTargetTypeCriteria>());

            // Assert
            Assert.AreEqual(expectedTargetTypes, actual);
        }

        private TargetTypeService ArrangeGetTargetType(IEnumerable<TargetTypeModel> targetTypes = null)
        {
            var targetTypeService = Substitute.For<TargetTypeService>();
            targetTypeService.GetData<TargetTypeModel>(Arg.Any<GetTargetTypeCriteria>()).Returns(targetTypes ?? new List<TargetTypeModel>());
            return targetTypeService;
        }
    }
}