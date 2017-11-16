namespace SMS.EPA.Criteria.UnitTests
{
    using Criteria;
    using NUnit.Framework;

    [TestFixture]
    public class GetPersonalFlowByFlowTemplateIdCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetPersonalFlowByFlowTemplateIdCriteria();
            string expected = "SettingEmployeeFlow_Get";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(1, true)]
        [TestCase(0, false)]
        public void IsValid_CheckParameter_ReturnAsExpected(int flowTemplateId, bool expected)
        {
            // Arrange
            var criteria = new GetPersonalFlowByFlowTemplateIdCriteria
            {
                FlowTemplateId = flowTemplateId
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}