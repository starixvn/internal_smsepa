namespace SMS.EPA.Criteria.UnitTests
{
    using NUnit.Framework;
    using TargetType;

    [TestFixture]
    public class GetTargetTypeCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetTargetTypeCriteria();
            string expected = "TargetType_Get";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        public void IsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetTargetTypeCriteria();

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.IsTrue(actual);
        }
    }
}