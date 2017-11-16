namespace SMS.EPA.Criteria.UnitTests
{
    using Criteria;
    using NUnit.Framework;

    [TestFixture]
    public class UpdateKPIGroupCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new UpdateKPIGroupCriteria();
            string expected = "KPIGroup_Update";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(1, "test", 1, true)]
        [TestCase(1, "test", 0, false)]
        public void IsValid_CheckParameter_ReturnAsExpected(int kpiGroupId, string name, int modifiedBy, bool expected)
        {
            // Arrange
            var criteria = new UpdateKPIGroupCriteria
            {
                KPIGroupId = kpiGroupId,
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