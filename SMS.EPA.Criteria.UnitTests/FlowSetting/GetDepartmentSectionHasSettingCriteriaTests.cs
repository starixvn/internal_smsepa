namespace SMS.EPA.Criteria.UnitTests
{
    using NUnit.Framework;
    using SMS.EPA.Criteria;

    [TestFixture]
    public class GetDepartmentSectionHasSettingCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetDepartmentSectionHasSettingCriteria();
            string expected = "FlowTemplate_GetDepartmentHasSetting";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IsValid_FlowTemplateIdGreaterThanZero_ReturnTrue()
        {
            // Arrange
            var criteria = new GetDepartmentSectionHasSettingCriteria()
            {
                FlowTemplateId = 1
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
            var criteria = new GetDepartmentSectionHasSettingCriteria()
            {
                FlowTemplateId = -1
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.IsFalse(actual);
        }
    }
}