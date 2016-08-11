namespace SMS.EPA.Criteria.UnitTests
{
    using NUnit.Framework;

    [TestFixture]
    public class UpdateStatusKPICriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new UpdateStatusKPICriteria();
            string expected = "KPI_UpdateStatus";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // return true if kpiId > 0 and modifiedBy > 0.
        // else return false
        [TestCase(1, 1, true)]
        [TestCase(0, 0, false)]
        [TestCase(1, 0, false)]
        [TestCase(0, 1, false)]
        public void IsValid_VariousValues_ChecksThem(int kpiId, int modifiedBy, bool expected)
        {
            // Arrange
            var criteria = new UpdateStatusKPICriteria
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