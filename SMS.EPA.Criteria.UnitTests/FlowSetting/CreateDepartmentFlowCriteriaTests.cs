namespace SMS.EPA.Criteria.UnitTests
{
    using Criteria;
    using NUnit.Framework;

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

        [TestCase(1, 1, 1, true)]
        [TestCase(0, 0, 0, false)]
        public void IsValid_CheckParameter_ReturnAsExpected(int flowTemplateId, int departmentId, int modifiedId, bool expected)
        {
            // Arrange
            var criteria = new CreateDepartmentFlowCriteria
            {
                FlowTemplateId = flowTemplateId,
                DepartmentId = departmentId,
                ModifiedBy = modifiedId
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}