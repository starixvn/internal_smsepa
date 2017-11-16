namespace SMS.EPA.Criteria.UnitTests
{
    using Criteria;
    using NUnit.Framework;

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

        [TestCase(1, 1, true)]
        [TestCase(0, 1, false)]
        public void IsValid_CheckParameter_ReturnAsExpected(
            int targetTemplateId,
            int modifiedBy,
            bool expected)
        {
            // Arrange
            var criteria = new AssignTargetDepartmentSettingCriteria
            {
                TargetTemplateId = targetTemplateId,
                ModifiedBy = modifiedBy
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}