namespace SMS.EPA.Criteria.UnitTests.Department
{
    using Criteria;
    using NUnit.Framework;

    [TestFixture]
    public class AssignTargetTemplateToEmployeeCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new AssignTargetTemplateToEmployeeCriteria();
            string expected = "EpaTargetTemplateEmployee_Assign";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(1, 1, true)]
        [TestCase(0, 1, false)]
        public void IsValid_CheckParameter_ReturnAsExpected(
            int targetTemplateId,
            int modifiedBy,
            bool expected)
        {
            // Arrange
            var criteria = new AssignTargetTemplateToEmployeeCriteria
            {
                TargetTemplateId = targetTemplateId,
                ModifiedBy = modifiedBy
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}