namespace SMS.EPA.Criteria.UnitTests
{
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

        // returns true if
        // kpiId > 0
        // and name, kpiGroupIds are not empty and not null and not consists only of white-space characters
        // and modifiedBy > 0.
        [TestCase(1, "test", "1", 1, true)]
        // else return false
        [TestCase(1, "test", "1", 0, false)]
        [TestCase(1, "test", "1", -1, false)]
        [TestCase(1, "", "", 0, false)]
        [TestCase(1, "test", "", 0, false)]
        [TestCase(1, "", "1", 0, false)]
        public void IsValid_VariousValues_ChecksThem(int kpiId, string name, string kpiGroupIds, int modifiedBy, bool expected)
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