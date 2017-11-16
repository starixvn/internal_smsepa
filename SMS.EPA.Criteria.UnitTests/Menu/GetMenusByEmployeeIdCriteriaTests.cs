namespace SMS.EPA.Criteria.UnitTests
{
    using Criteria;
    using NUnit.Framework;

    [TestFixture]
    public class GetMenusByEmployeeIdCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetMenusByEmployeeIdCriteria();
            string expected = "Menu_GetByEmployeeId";

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
            var criteria = new GetMenusByEmployeeIdCriteria
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