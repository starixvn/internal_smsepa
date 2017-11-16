namespace SMS.EPA.Criteria.UnitTests
{
    using Criteria;
    using NUnit.Framework;

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

        [TestCase(1, 1, true)]
        [TestCase(0, 0, false)]
        public void IsValid_CheckParameter_ReturnAsExpected(int flowTemplateId, int modifiedBy, bool expected)
        {
            // Arrange
            var criteria = new UpdateStatusFlowTemplateCriteria
            {
                FlowTemplateId = flowTemplateId,
                ModifiedBy = modifiedBy
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}