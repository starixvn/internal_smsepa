namespace SMS.EPA.Criteria.UnitTests
{
    using NUnit.Framework;
    using SMS.EPA.Criteria;

    [TestFixture]
    public class CreateDepartmentFlowCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CreateDepartmentFlowCriteria();
            string expected = "SettingDepartmentFlow_Set";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IsValid_ModifiedByAndDepartmentIdAndFlowTemplateIdGreaterThanZero_ReturnTrue()
        {
            // Arrange
            var criteria = new CreateDepartmentFlowCriteria()
            {
                FlowTemplateId = 1,
                DepartmentId = 1,
                ModifiedBy = 1
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
            var criteria = new CreateDepartmentFlowCriteria()
            {
                DepartmentId = -1,
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
            var criteria = new CreateDepartmentFlowCriteria()
            {
                FlowTemplateId = -1,
                DepartmentId = 1,
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
            var criteria = new CreateDepartmentFlowCriteria()
            {
                FlowTemplateId = 1,
                DepartmentId = 1,
                ModifiedBy = -1
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.IsFalse(actual);
        }
    }
}