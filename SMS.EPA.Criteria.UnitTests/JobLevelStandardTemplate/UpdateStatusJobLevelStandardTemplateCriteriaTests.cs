namespace SMS.EPA.Criteria.UnitTests.JobLevelStandardTemplate
{
    using NUnit.Framework;
    using SMS.EPA.Criteria.JobLevelStandard.Template;

    [TestFixture]
    public class UpdateStatusJobLevelStandardTemplateCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new UpdateStatusJobLevelStandardTemplateCriteria();
            string expected = "JobLevelStandard_Template_UpdateStatus";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(1, 1, true)]
        [TestCase(0, 0, false)]
        public void IsValid_CheckParameter_ReturnAsExpected(int templateId, int modifiedBy, bool expected)
        {
            // Arrange
            var criteria = new UpdateStatusJobLevelStandardTemplateCriteria
            {
                TemplateId = templateId,
                ModifiedBy = modifiedBy
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
