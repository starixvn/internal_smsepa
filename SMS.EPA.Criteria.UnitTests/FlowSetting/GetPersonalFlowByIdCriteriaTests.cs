namespace SMS.EPA.Criteria.UnitTests.Department
{
    using NUnit.Framework;
    using SMS.EPA.Criteria.TargetEmployeeSetting;

    [TestFixture]
    public class GetPersonalFlowByIdCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetPersonalFlowByIdCriteria();
            string expected = "SettingEmployeeFlow_GetById";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IsValid_EmployeeIdFlowAndTemplateIdGreaterThanZero_ReturnTrue()
        {
            // Arrange
            var criteria = new GetPersonalFlowByIdCriteria() 
            {
                FlowTemplateId = 1,
                EmployeeId = 1
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void IsValid_FlowTemplateIdLessThanZero_ReturnFalse()
        {
            // Arrange
            var criteria = new GetPersonalFlowByIdCriteria()
            {
                FlowTemplateId = -1,
                EmployeeId = 1
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.IsFalse(actual);
        }

        [Test]
        public void IsValid_EmployeeIdLessThanZero_ReturnFalse()
        {
            // Arrange
            var criteria = new GetPersonalFlowByIdCriteria()
            {
                FlowTemplateId = 1,
                EmployeeId = -1
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.IsFalse(actual);
        }
    }
}
