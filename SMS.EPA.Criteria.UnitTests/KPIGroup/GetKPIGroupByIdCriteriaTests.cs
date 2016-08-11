namespace SMS.EPA.Criteria.UnitTests
{
    using NUnit.Framework;

    [TestFixture]
    public class GetKPIGroupByIdCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new GetKPIGroupByIdCriteria();
            string expected = "KPIGroup_GetById";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // return true if kpiGroupId > 0
        // else return false
        [TestCase(1, true)]
        [TestCase(0, false)]
        public void IsValid_VariousValues_ChecksThem(int kpiGroupId, bool expected)
        {
            // Arrange
            var criteria = new GetKPIGroupByIdCriteria
            {
                KPIGroupId = kpiGroupId
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}