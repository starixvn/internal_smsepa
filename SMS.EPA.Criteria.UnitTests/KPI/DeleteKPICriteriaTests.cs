namespace SMS.EPA.Criteria.UnitTests
{
    using NUnit.Framework;

    [TestFixture]
    public class DeleteKPICriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new DeleteKPICriteria();
            string expected = "KPI_Delete";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // returns true if kpiId > 0 and ModifiedBy > 0
        [TestCase(1, 1, true)]
        //// else return false
        [TestCase(0, 1, false)]
        [TestCase(1, 0, false)]
        [TestCase(0, 0, false)]
        public void IsValid_VariousValues_ChecksThem(int kpiId, int modifiedBy, bool expected)
        {
            // Arrange
            var criteria = new DeleteKPICriteria
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