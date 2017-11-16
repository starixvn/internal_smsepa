namespace SMS.EPA.Criteria.UnitTests.Dashboard
{
    using Criteria;
    using NUnit.Framework;

    [TestFixture]
    public class GetToDoListCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetToDoListCriteria();
            string expected = "Dashboard_GetToDoList";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(1, true)]
        [TestCase(0, false)]
        public void IsValid_CheckParameter_ReturnAsExpected(int employeeId, bool expected)
        {
            // Arrange
            var criteria = new GetToDoListCriteria
            {
                EmployeeId = employeeId
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}