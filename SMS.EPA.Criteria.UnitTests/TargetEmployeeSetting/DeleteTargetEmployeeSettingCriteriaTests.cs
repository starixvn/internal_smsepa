namespace SMS.EPA.Criteria.UnitTests
{
    using Criteria;
    using NUnit.Framework;

    [TestFixture]
    public class DeleteTargetEmployeeSettingCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new DeleteTargetEmployeeSettingCriteria();
            string expected = "EpaTargetTemplateEmployee_Delete";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(1, 1, 1, true)]
        [TestCase(0, 0, 1, false)]
        public void IsValid_CheckParameter_ReturnAsExpected(
            int targetTemplateId,
            int employeeId,
            int modifiedBy,
            bool expected)
        {
            // Arrange
            var criteria = new DeleteTargetEmployeeSettingCriteria
            {
                TargetTemplateId = targetTemplateId,
                EmployeeId = employeeId,
                ModifiedBy = modifiedBy
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}