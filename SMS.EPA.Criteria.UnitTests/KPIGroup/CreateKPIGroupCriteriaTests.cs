namespace SMS.EPA.Criteria.UnitTests
{
    using Criteria;
    using NUnit.Framework;

    [TestFixture]
    public class CreateKPIGroupCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CreateKPIGroupCriteria();
            string expected = "KPIGroup_Insert";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase("test", 1, true)]
        [TestCase("test", 0, false)]
        public void IsValid_CheckParameter_ReturnAsExpected(string name, int createdBy, bool expected)
        {
            // Arrange
            var criteria = new CreateKPIGroupCriteria
            {
                Name = name,
                CreatedBy = createdBy
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}