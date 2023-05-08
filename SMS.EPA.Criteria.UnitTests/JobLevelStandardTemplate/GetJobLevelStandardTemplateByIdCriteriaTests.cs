namespace SMS.EPA.Criteria.UnitTests.JobLevelStandardTemplate
{
    using NUnit.Framework;
    using SMS.EPA.Criteria.JobLevelStandard.Template;

    [TestFixture]
    public class GetJobLevelStandardTemplateByIdCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetJobLevelStandardTemplateByIdCriteria();
            string expected = "JobLevelStandard_Template_GetById";

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
            var criteria = new GetJobLevelStandardTemplateByIdCriteria
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
