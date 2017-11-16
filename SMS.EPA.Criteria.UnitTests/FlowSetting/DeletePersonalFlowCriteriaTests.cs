namespace SMS.EPA.Criteria.UnitTests
{
    using Criteria;
    using NUnit.Framework;

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

        [TestCase(1, 1, 1, true)]
        [TestCase(0, 0, 0, false)]
        public void IsValid_CheckParameter_ReturnAsExpected(int flowTemplateId, int employeeId, int modifiedId, bool expected)
        {
            // Arrange
            var criteria = new DeletePersonalFlowCriteria
            {
                FlowTemplateId = flowTemplateId,
                EmployeeId = employeeId,
                ModifiedBy = modifiedId
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}