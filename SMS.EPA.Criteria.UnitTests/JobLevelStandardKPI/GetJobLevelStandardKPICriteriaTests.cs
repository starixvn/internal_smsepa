namespace SMS.EPA.Criteria.UnitTests.JobLevelStandardKPI
{
    using NUnit.Framework;
    using SMS.EPA.Criteria.JobLevelStandard.KPI;

    [TestFixture]
    public class GetJobLevelStandardKPICriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetJobLevelStandardKPICriteria();
            string expected = "JobLevelStandard_KPI_Get";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetJobLevelStandardKPICriteria();

            // Act
            var actual = criteria.IsValid();

            // Assert
            Assert.IsTrue(actual);
        }
    }
}
