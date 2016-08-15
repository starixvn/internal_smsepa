namespace SMS.EPA.Criteria.UnitTests
{
    using NUnit.Framework;

    [TestFixture]
    public class GetFunctionByRoleIdCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetFunctionByRoleIdCriteria();
            string expected = "EpaRole_GetFunctionByRoleId";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // return true if roleId > 0
        // else return false
        [TestCase(1, true)]
        [TestCase(0, false)]
        [TestCase(-1, false)]
        public void IsValid_VariousValues_ChecksThem(int roleId, bool expected)
        {
            // Arrange
            var criteria = new GetFunctionByRoleIdCriteria
            {
                RoleId = roleId
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}