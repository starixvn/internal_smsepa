namespace SMS.EPA.Criteria.UnitTests
{
    using NUnit.Framework;
    using SMS.EPA.Criteria;

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

        [Test]
        public void IsValid_TargetTemplateIdGreaterThanZero_ReturnTrue()
        {
            // Arrange
            var criteria = new DeleteTargetEmployeeSettingCriteria()
            {
                TargetTemplateId = 1,
                EmployeeId = 1
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
            var criteria = new DeleteTargetEmployeeSettingCriteria()
            {
                TargetTemplateId = -1,
                EmployeeId = 1
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.IsFalse(actual);
        }

        [Test]
        public void IsValid_EmployeeIdGreaterThanZero_ReturnTrue()
        {
            // Arrange
            var criteria = new DeleteTargetEmployeeSettingCriteria()
            {
                TargetTemplateId = 1,
                EmployeeId = 1
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void IsValid_EmployeeIdLessThanZero_ReturnFalse()
        {
            // Arrange
            var criteria = new DeleteTargetEmployeeSettingCriteria()
            {
                TargetTemplateId = 1,
                EmployeeId = -1
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.IsFalse(actual);
        }
    }
}