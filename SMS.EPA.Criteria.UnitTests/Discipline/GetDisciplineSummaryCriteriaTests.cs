namespace SMS.EPA.Criteria.UnitTests
{
    using System;
    using Criteria;
    using NUnit.Framework;

    [TestFixture]
    public class GetDisciplineSummaryCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetDisciplineSummaryCriteria();
            var expected = "DisciplineSummary_Get";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IsValid_InvalidParameter_ReturnFalse()
        {
            // Arrange
            var criteria = new GetDisciplineSummaryCriteria
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
            var criteria = new GetDisciplineSummaryCriteria
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                ApproverId = 1
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.IsTrue(actual);
        }
    }
}