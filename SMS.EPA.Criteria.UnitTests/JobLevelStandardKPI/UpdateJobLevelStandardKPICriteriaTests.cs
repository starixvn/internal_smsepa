namespace SMS.EPA.Criteria.UnitTests.JobLevelStandardKPI
{
    using Criteria;
    using NUnit.Framework;

    [TestFixture]
    public class UpdateJobLevelStandardKPICriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new UpdateJobLevelStandardKPICriteria();
            string expected = "JobLevelStandard_KPI_Update";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(1, "test", 1, true)]
        [TestCase(1, "test", 0, false)]
        public void IsValid_CheckParameter_ReturnAsExpected(int kpiId, string name, int modifiedBy, bool expected)
        {
            // Arrange
            var criteria = new UpdateJobLevelStandardKPICriteria
            {
                KPIId = kpiId,
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
