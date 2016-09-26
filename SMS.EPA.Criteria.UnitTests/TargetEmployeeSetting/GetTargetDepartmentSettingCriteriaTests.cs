namespace SMS.EPA.Criteria.UnitTests.Department
{
    using NUnit.Framework;
    using SMS.EPA.Criteria.TargetEmployeeSetting;

    [TestFixture]
    public class GetTargetDepartmentSettingCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetTargetDepartmentSettingCriteria();
            string expected = "EpaTargetTemplateDepartment_Get";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetTargetDepartmentSettingCriteria();

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.IsTrue(actual);
        }
    }
}
