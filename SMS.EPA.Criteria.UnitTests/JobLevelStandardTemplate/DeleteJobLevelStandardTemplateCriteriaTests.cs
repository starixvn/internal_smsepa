namespace SMS.EPA.Criteria.UnitTests.JobLevelStandardTemplate
{
    using NUnit.Framework;
    using SMS.EPA.Criteria.JobLevelStandard.Template;

    [TestFixture]
    public class DeleteJobLevelStandardTemplateCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new DeleteJobLevelStandardTemplateCriteria();
            string expected = "JobLevelStandard_Template_Delete";

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
            var criteria = new DeleteJobLevelStandardTemplateCriteria
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
