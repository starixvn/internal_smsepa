namespace SMS.EPA.Criteria.UnitTests
{
    using Criteria;
    using NUnit.Framework;

    [TestFixture]
    public class UpdateKPICriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new UpdateKPICriteria();
            string expected = "KPI_Update";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(1, "test", "1", 1, true)]
        [TestCase(1, "test", "1", 0, false)]
        public void IsValid_CheckParameter_ReturnAsExpected(int kpiId, string name, string kpiGroupIds, int modifiedBy, bool expected)
        {
            // Arrange
            var criteria = new UpdateKPICriteria
            {
                KPIId = kpiId,
                Name = name,
                KPIGroupIds = kpiGroupIds,
                ModifiedBy = modifiedBy
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}