namespace SMS.EPA.Criteria.UnitTests
{
    using NUnit.Framework;

    [TestFixture]
    public class DeleteKPIGroupCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new DeleteKPIGroupCriteria();
            string expected = "KPIGroup_Delete";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // returns true if kpiGroupId > 0 and ModifiedBy > 0
        // else return false
        [TestCase(1, 1, true)]
        [TestCase(0, 1, false)]
        [TestCase(1, 0, false)]
        [TestCase(0, 0, false)]
        public void IsValid_VariousValues_ChecksThem(int kpiGroupId, int modifiedBy, bool expected)
        {
            // Arrange
            var criteria = new DeleteKPIGroupCriteria
            {
                KPIGroupId = kpiGroupId,
                ModifiedBy = modifiedBy
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}