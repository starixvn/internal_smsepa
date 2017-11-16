namespace SMS.EPA.Criteria.UnitTests
{
    using Criteria;
    using NUnit.Framework;

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

        [TestCase(1, 1, true)]
        [TestCase(0, 0, false)]
        public void IsValid_CheckParameter_ReturnAsExpected(int flowTemplateId, int departmentId, bool expected)
        {
            // Arrange
            var criteria = new GetDepartmentFlowByIdCriteria
            {
                FlowTemplateId = flowTemplateId,
                DepartmentId = departmentId
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}