namespace SMS.EPA.Criteria.UnitTests
{
    using Criteria;
    using NUnit.Framework;

    [TestFixture]
    public class CreatePersonalFlowCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CreatePersonalFlowCriteria();
            string expected = "SettingEmployeeFlow_Set";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(1, "1", 1, true)]
        [TestCase(0, "", 0, false)]
        public void IsValid_CheckParameter_ReturnAsExpected(int flowTemplateId, string employeeIds, int modifiedId, bool expected)
        {
            // Arrange
            var criteria = new CreatePersonalFlowCriteria
            {
                FlowTemplateId = flowTemplateId,
                ModifiedBy = modifiedId,
                EmployeeIds = employeeIds
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}