namespace SMS.EPA.Criteria.UnitTests
{
    using Criteria;
    using NUnit.Framework;

    [TestFixture]
    public class AssignFunctionToRoleCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new AssignFunctionToRoleCriteria();
            string expected = "EpaRole_AssignFunctionToRole";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestCase(1, 1, "<ROOT><Function FunctionId=\"1\" IsRead=\"1\" IsHost=\"0\"></Function></ROOT>", true)]
        [TestCase(1, 1, "", false)]
        public void IsValid_CheckParameter_ReturnAsExpected(int roleId, int modifiedBy, string functions, bool expected)
        {
            // Arrange
            var criteria = new AssignFunctionToRoleCriteria
            {
                RoleId = roleId,
                Functions = functions,
                ModifiedBy = modifiedBy
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}