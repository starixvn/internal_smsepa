namespace SMS.EPA.Criteria.UnitTests.Department
{
    using NUnit.Framework;
    using SMS.EPA.Criteria.TargetEmployeeSetting;
    using SMS.EPA.Model;
    using System.Collections.Generic;

    [TestFixture]
    public class CreatePersonalFlowCriteriaTests
    {
        [Test]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var criteria = new CreatePersonalFlowCriteria();
            string expected = "SettingEmployeeFlow_Set";

            // Act
            string actual = criteria.GetSettingKey();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IsValid_ModifiedByAndEmployeeIdAndFlowTemplateIdGreaterThanZero_ReturnTrue()
        {
            // Arrange
            var criteria = new CreatePersonalFlowCriteria() 
            {
                FlowTemplateId = 1,
                ModifiedBy = 1,
                Selfs = GetSelf(employeeId: 1)
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void IsValid_EmployeeIdLessThanZero_ReturnFalse()
        {
            // Arrange
            var criteria = new CreatePersonalFlowCriteria()
            {
               Selfs = GetSelf(employeeId: -1),
                FlowTemplateId = 1,
                ModifiedBy = 1
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.IsFalse(actual);
        }

        [Test]
        public void IsValid_FlowTemplateIdLessThanZero_ReturnFalse()
        {
            // Arrange
            var criteria = new CreatePersonalFlowCriteria()
            {
                FlowTemplateId = -1,
                Selfs = GetSelf(employeeId: 1),
                ModifiedBy = 1
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.IsFalse(actual);
        }

        [Test]
        public void IsValid_ModifiedByLessThanZero_ReturnFalse()
        {
            // Arrange
            var criteria = new CreatePersonalFlowCriteria()
            {
                FlowTemplateId = 1,
                Selfs = GetSelf(employeeId: 1),
                ModifiedBy = -1
            };

            // Act
            bool actual = criteria.IsValid();

            // Assert
            Assert.IsFalse(actual);
        }

        private FlowItemModel GetSelf(int employeeId)
        {
            var flowItem = new FlowItemModel();
            var employees = new List<EvaluatorInfoViewModel>();
            employees.Add(new EvaluatorInfoViewModel() { EmployeeId = employeeId });
            flowItem.Employees = employees;
            return flowItem;
        }
    }
}
