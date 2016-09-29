namespace SMS.EPA.Criteria.UnitTests.Department
{
    using NUnit.Framework;
    using SMS.EPA.Criteria.TargetEmployeeSetting;

    [TestFixture]
    public class DeletePersonalFlowCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new DeletePersonalFlowCriteria();
            string expected = "SettingEmployeeFlow_Delete";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IsValid_ModifiedByAndEmployeeIdAndFlowTemplateIdGreaterThanZero_ReturnTrue()
        {
            // Arrange
            var criteria = new DeletePersonalFlowCriteria() 
            {
                FlowTemplateId = 1,
                EmployeeId = 1,
                ModifiedBy = 1
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
            var criteria = new DeletePersonalFlowCriteria()
            {
                EmployeeId = -1,
                FlowTemplateId = 1,
                ModifiedBy = 1
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.IsFalse(actual);
        }

        [Test]
        public void IsValid_FlowTemplateIdLessThanZero_ReturnFalse()
        {
            // Arrange
            var criteria = new DeletePersonalFlowCriteria()
            {
                FlowTemplateId = -1,
                EmployeeId = 1,
                ModifiedBy = 1
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.IsFalse(actual);
        }

        [Test]
        public void IsValid_ModifiedByLessThanZero_ReturnFalse()
        {
            // Arrange
            var criteria = new DeletePersonalFlowCriteria()
            {
                FlowTemplateId = 1,
                EmployeeId = 1,
                ModifiedBy = -1
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.IsFalse(actual);
        }
    }
}
