namespace SMS.EPA.Criteria.UnitTests.JobLevelStandardTemplate
{
    using NUnit.Framework;
    using SMS.EPA.Criteria.JobLevelStandard.Template;

    [TestFixture]
    public class GetKPIListSelectedCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetKPIListCriteria();
            string expected = "JobLevelStandard_Template_GetKPIList";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(1, true)]
        [TestCase(0, false)]
        public void IsValid_CheckParameter_ReturnAsExpected(int templateId, bool expected)
        {
            // Arrange
            var criteria = new GetKPIListCriteria
            {
                TemplateId = templateId,
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
