namespace SMS.EPA.Criteria.UnitTests.Department
{
    using NUnit.Framework;
    using SMS.EPA.Criteria;

    [TestFixture]
    public class AssignTargetTemplateToEmployeeCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new AssignTargetTemplateToEmployeeCriteria();
            string expected = "EpaTargetTemplateEmployee_Assign";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IsValid_TargetTemplateIdGreaterThanZero_ReturnTrue()
        {
            // Arrange
            var criteria = new AssignTargetTemplateToEmployeeCriteria()
            {
                TargetTemplateId = 1
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void IsValid_TargetTemplateIdLessThanZero_ReturnFalse()
        {
            // Arrange
            var criteria = new AssignTargetTemplateToEmployeeCriteria()
            {
                TargetTemplateId = -1
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.IsFalse(actual);
        }
    }
}