namespace SMS.EPA.Criteria.UnitTests
{
    using Criteria;
    using NUnit.Framework;

    [TestFixture]
    public class GetScoreRatingTypeCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetScoreRatingTypeCriteria();
            string expected = "EpaScoreRatingType_Get";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        public void IsValid_ReturnTrue()
        {
            // Arrange
            var criteria = new GetScoreRatingTypeCriteria();

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.IsTrue(actual);
        }
    }
}