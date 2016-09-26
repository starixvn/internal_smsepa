namespace SMS.EPA.Criteria.UnitTests.Department
{
    using NUnit.Framework;
    using SMS.EPA.Criteria.TargetEmployeeSetting;

    [TestFixture]
    public class AssignTargetDepartmentSettingCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new AssignTargetDepartmentSettingCriteria();
            string expected = "EpaTargetTemplateDepartment_Assign";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IsValid_TargetTemplateIdGreaterThanZero_ReturnTrue()
        {
            // Arrange
            var criteria = new AssignTargetDepartmentSettingCriteria()
            {
                TargetTemplateId = 1
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void IsValid_TargetTemplateIdLessThanZero_ReturnFalse()
        {
            // Arrange
            var criteria = new AssignTargetDepartmentSettingCriteria()
            {
                TargetTemplateId = -1
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.IsFalse(actual);
        }
    }
}
