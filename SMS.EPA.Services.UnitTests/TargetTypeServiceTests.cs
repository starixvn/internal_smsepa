namespace SMS.EPA.Services.UnitTests
{
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Criteria.TargetType;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Services.Implement;
    using System.Collections.Generic;

    [TestFixture]
    public class TargetTypeServiceTests
    {
        [Test]
        public void GetTargetType_AnyArg_ReturnExpectedTargetTypes()
        {
            // Arrange
            var expectedTargetTypes = new List<TargetType>();
            expectedTargetTypes.Add(new TargetType());
            var targetTypeService = ArrangeGetTargetType(expectedTargetTypes);

            // Act
            var actual = targetTypeService.GetTargetType(Arg.Any<GetTargetTypeCriteria>());

            // Assert
            Assert.AreEqual(expectedTargetTypes, actual);
        }

        private TargetTypeService ArrangeGetTargetType(IEnumerable<TargetType> targetTypes = null)
        {
            var targetTypeService = Substitute.For<TargetTypeService>();
            targetTypeService.GetData<TargetType>(Arg.Any<GetTargetTypeCriteria>()).Returns(targetTypes ?? new List<TargetType>());
            return targetTypeService;
        }
    }
}
