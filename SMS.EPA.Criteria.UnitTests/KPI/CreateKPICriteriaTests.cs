namespace SMS.EPA.Criteria.UnitTests
{
    using NUnit.Framework;

    [TestFixture]
    public class CreateKPICriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CreateKPICriteria();
            string expected = "KPI_Insert";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // return true if
        // name and kpiGroupIds are not empty and not null and not consists only of white-space characters
        // and createdBy > 0.
        // else return false
        [TestCase("test", "1", 1, true)]
        [TestCase("test", "1", 0, false)]
        [TestCase("test", "1", -1, false)]
        [TestCase("", "", 0, false)]
        [TestCase("test", "", 0, false)]
        [TestCase("", "1", 0, false)]
        public void IsValid_VariousValues_ChecksThem(string name, string kpiGroupIds, int createdBy, bool expected)
        {
            // Arrange
            var criteria = new CreateKPICriteria
            {
                Name = name,
                KPIGroupIds = kpiGroupIds,
                CreatedBy = createdBy
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}