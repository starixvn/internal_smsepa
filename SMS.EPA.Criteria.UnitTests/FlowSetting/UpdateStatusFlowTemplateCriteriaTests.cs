namespace SMS.EPA.Criteria.UnitTests.Department
{
    using NUnit.Framework;
    using SMS.EPA.Criteria.TargetEmployeeSetting;

    [TestFixture]
    public class UpdateStatusFlowTemplateCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new UpdateStatusFlowTemplateCriteria();
            string expected = "FlowTemplate_UpdateStatus";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IsValid_ModifiedByAndFlowTemplateIdGreaterThanZero_ReturnTrue()
        {
            // Arrange
            var criteria = new UpdateStatusFlowTemplateCriteria() 
            {
                FlowTemplateId = 1,
                ModifiedBy = 1
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
            var criteria = new UpdateStatusFlowTemplateCriteria()
            {
                FlowTemplateId = -1,
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
            var criteria = new UpdateStatusFlowTemplateCriteria()
            {
                FlowTemplateId = 1,
                ModifiedBy = -1
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.IsFalse(actual);
        }
    }
}
