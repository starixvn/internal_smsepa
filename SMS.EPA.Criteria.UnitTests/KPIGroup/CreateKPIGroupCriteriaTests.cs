namespace SMS.EPA.Criteria.UnitTests
{
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

        // return true if name is not empty and not null and not consists only of white-space characters
        // and createdBy > 0
        // else return false
        [Test]
        [TestCase("test", 1, true)]
        [TestCase("test", 0, false)]
        [TestCase(null, 1, false)]
        [TestCase(null, 0, false)]
        public void IsValid_VariousValues_ChecksThem(string name, int createdBy, bool expected)
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