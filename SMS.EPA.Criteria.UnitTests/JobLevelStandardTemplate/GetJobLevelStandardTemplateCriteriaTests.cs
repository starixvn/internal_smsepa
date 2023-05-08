namespace SMS.EPA.Criteria.UnitTests.JobLevelStandardTemplate
{
    using NUnit.Framework;
    using SMS.EPA.Criteria.JobLevelStandard.Template;

    [TestFixture]
    public class GetJobLevelStandardTemplateCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetJobLevelStandardTemplateCriteria();
            string expected = "JobLevelStandard_Template_Get";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetJobLevelStandardTemplateCriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.IsTrue(actual);
        }
    }
}
