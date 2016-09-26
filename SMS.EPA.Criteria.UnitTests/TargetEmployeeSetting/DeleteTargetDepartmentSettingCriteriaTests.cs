namespace SMS.EPA.Criteria.UnitTests.Department
{
    using NUnit.Framework;
    using SMS.EPA.Criteria.TargetEmployeeSetting;

    [TestFixture]
    public class DeleteTargetDepartmentSettingCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new DeleteTargetDepartmentSettingCriteria();
            string expected = "EpaTargetTemplateDepartment_Delete";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IsValid_TargetTemplateIdGreaterThanZero_ReturnTrue()
        {
            // Arrange
            var criteria = new DeleteTargetDepartmentSettingCriteria()
            {
                TargetTemplateId = 1,
                DepartmentId = 1
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
            var criteria = new DeleteTargetDepartmentSettingCriteria()
            {
                TargetTemplateId = -1,
                DepartmentId = 1
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.IsFalse(actual);
        }

        [Test]
        public void IsValid_DepartmentIdGreaterThanZero_ReturnTrue()
        {
            // Arrange
            var criteria = new DeleteTargetDepartmentSettingCriteria()
            {
                TargetTemplateId = 1,
                DepartmentId = 1
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void IsValid_DepartmentIdLessThanZero_ReturnFalse()
        {
            // Arrange
            var criteria = new DeleteTargetDepartmentSettingCriteria()
            {
                TargetTemplateId = 1,
                DepartmentId = -1
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.IsFalse(actual);
        }
    }
}
