namespace SMS.EPA.Criteria.UnitTests
{
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

        // return true if
        // kpiGroupId > 0
        // and name is not empty and not null and not consists only of white-space characters
        // and modifiedBy > 0.
        // else return false
        [TestCase(1, "test", 1, true)]
        [TestCase(1, "test", 0, false)]
        [TestCase(1, "", 1, false)]
        [TestCase(1, "", 0, false)]
        [TestCase(0, "test", 1, false)]
        [TestCase(0, "test", 0, false)]
        public void IsValid_VariousValues_ChecksThem(int kpiGroupId, string name, int modifiedBy, bool expected)
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