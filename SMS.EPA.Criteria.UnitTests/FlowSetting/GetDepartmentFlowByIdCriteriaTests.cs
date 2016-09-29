namespace SMS.EPA.Criteria.UnitTests.Department
{
    using NUnit.Framework;
    using SMS.EPA.Criteria.TargetEmployeeSetting;

    [TestFixture]
    public class GetDepartmentFlowByIdCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetDepartmentFlowByIdCriteria();
            string expected = "SettingDepartmentFlow_GetById";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IsValid_DepartmentIdAndFlowTemplateIdGreaterThanZero_ReturnTrue()
        {
            // Arrange
            var criteria = new GetDepartmentFlowByIdCriteria() 
            {
                DepartmentId = 1,
                FlowTemplateId = 1
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
            var criteria = new GetDepartmentFlowByIdCriteria()
            {
                DepartmentId = -1,
                FlowTemplateId = 1
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
            var criteria = new GetDepartmentFlowByIdCriteria()
            {
                DepartmentId = 1,
                FlowTemplateId = -1
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.IsFalse(actual);
        }
    }
}
