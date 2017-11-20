namespace SMS.EPA.Criteria.UnitTests.Discipline
{
    using System;
    using Criteria;
    using NUnit.Framework;

    [TestFixture]
    public class GetDisciplineSummaryForOtherSystemCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetDisciplineSummaryForOtherSystemCriteria();
            var expected = "DisciplineSummary_GetForOtherSystem";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IsValid_InvalidParameter_ReturnFalse()
        {
            // Arrange
            var criteria = new GetDisciplineSummaryForOtherSystemCriteria
            {
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.IsFalse(actual);
        }

        [Test]
        public void IsValid_ValidParameter_ReturnTrue()
        {
            // Arrange
            var criteria = new GetDisciplineSummaryForOtherSystemCriteria
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.IsTrue(actual);
        }
    }
}