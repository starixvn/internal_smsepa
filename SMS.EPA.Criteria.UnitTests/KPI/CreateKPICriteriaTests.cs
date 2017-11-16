namespace SMS.EPA.Criteria.UnitTests
{
    using Criteria;
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

        [TestCase("test", "1", 1, true)]
        [TestCase("test", "1", 0, false)]
        public void IsValid_CheckParameter_ReturnAsExpected(string name, string kpiGroupIds, int createdBy, bool expected)
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