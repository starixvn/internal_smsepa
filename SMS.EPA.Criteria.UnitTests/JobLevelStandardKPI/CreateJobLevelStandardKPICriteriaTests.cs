namespace SMS.EPA.Criteria.UnitTests.JobLevelStandardKPI
{
    using Criteria;
    using NUnit.Framework;

    [TestFixture]
    public class CreateJobLevelStandardKPICriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CreateJobLevelStandardKPICriteria();
            string expected = "JobLevelStandard_KPI_Insert";

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
            var criteria = new CreateJobLevelStandardKPICriteria
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
