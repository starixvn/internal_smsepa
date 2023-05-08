namespace SMS.EPA.Criteria.UnitTests.JobLevelStandardTemplate
{
    using NUnit.Framework;
    using SMS.EPA.Criteria.JobLevelStandard.Template;

    [TestFixture]
    public class CreateJobLevelStandardTemplateCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CreateJobLevelStandardTemplateCriteria();
            string expected = "JobLevelStandard_Template_Insert";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase("test", 1, true)]
        [TestCase("test", 0, false)]
        public void IsValid_CheckParameter_ReturnAsExpected(string name, int createdBy, bool expected)
        {
            // Arrange
            var criteria = new CreateJobLevelStandardTemplateCriteria
            {
                Name = name,
                CreatedBy = createdBy
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
