namespace SMS.EPA.Criteria.UnitTests.JobLevelStandardTemplate
{
    using NUnit.Framework;
    using SMS.EPA.Criteria.JobLevelStandard.Template;

    [TestFixture]
    public class UpdateJobLevelStandardTemplateCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new UpdateJobLevelStandardTemplateCriteria();
            string expected = "JobLevelStandard_Template_Update";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(1, "test", 1, true)]
        [TestCase(1, "test", 0, false)]
        public void IsValid_CheckParameter_ReturnAsExpected(int templateId, string name, int modifiedBy, bool expected)
        {
            // Arrange
            var criteria = new UpdateJobLevelStandardTemplateCriteria
            {
                TemplateId = templateId,
                Name = name,
                ModifiedBy = modifiedBy
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
