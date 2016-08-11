namespace SMS.EPA.Criteria.UnitTests
{
    using NUnit.Framework;

    [TestFixture]
    public class GetKPIGroupCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetKPIGroupCriteria();
            string expected = "KPIGroup_Get";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}