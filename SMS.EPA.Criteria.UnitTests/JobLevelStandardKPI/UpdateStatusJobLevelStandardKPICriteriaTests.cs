

namespace SMS.EPA.Criteria.UnitTests.JobLevelStandardKPI
{
    using Criteria;
    using NUnit.Framework;

    [TestFixture]
    public class UpdateStatusJobLevelStandardKPICriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new UpdateStatusJobLevelStandardKPICriteria();
            string expected = "JobLevelStandard_KPI_UpdateStatus";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(1, 1, true)]
        [TestCase(0, 0, false)]
        public void IsValid_CheckParameter_ReturnAsExpected(int kpiId, int modifiedBy, bool expected)
        {
            // Arrange
            var criteria = new UpdateStatusJobLevelStandardKPICriteria
            {
                KPIId = kpiId,
                ModifiedBy = modifiedBy
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
