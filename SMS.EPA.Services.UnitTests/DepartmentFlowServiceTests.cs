namespace SMS.EPA.Services.UnitTests
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Repositories;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Repositories;
    using SMS.EPA.Services.Implement;

    [TestFixture]
    public class DepartmentFlowServiceTests
    {
        private DepartmentFlowService departmentFlowService;
        private IFlowSettingRepository flowSettingRepository;

        #region GetDepartmentFlowByFlowTemplateId

        [Test]
        public void GetDepartmentFlowByFlowTemplateId_AnyArg_ReturnInstanceOfListOfDepartmentFlowModel()
        {
            // Arrange
            ArrangeGetDepartmentFlowByFlowTemplateId();

            // Act
            var actual = departmentFlowService.GetDepartmentFlowByFlowTemplateId(0);

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<DepartmentFlowModel>), actual);
        }

        [Test]
        public void GetDepartmentFlowByFlowTemplateId_VaidArg_ReturnExpectedDepartmentFlows()
        {
            // Arrange
            int flowTemplateId = 1;
            var expectedDepartmentFlows = GetListOfDepartmentFlowForTest();
            ArrangeGetDepartmentFlowByFlowTemplateId(expectedDepartmentFlows);

            // Act
            var actual = departmentFlowService.GetDepartmentFlowByFlowTemplateId(flowTemplateId);

            // Assert
            Assert.AreEqual(expectedDepartmentFlows, actual);
        }

        [Test]
        public void GetDepartmentFlowByFlowTemplateId_InVaidArg_ReturnEmptyDepartmentFlows()
        {
            // Arrange
            int flowTemplateId = 0;
            var expectedDepartmentFlows = GetListOfDepartmentFlowForTest();
            ArrangeGetDepartmentFlowByFlowTemplateId(expectedDepartmentFlows);

            // Act
            var actual = departmentFlowService.GetDepartmentFlowByFlowTemplateId(flowTemplateId);

            // Assert
            Assert.IsEmpty(actual);
        }

        #endregion GetDepartmentFlowByFlowTemplateId

        #region GetDepartmentFlowById

        [Test]
        public void GetDepartmentFlowById_AnyArg_ReturnInstanceOfListOfDepartmentFlowModel()
        {
            // Arrange
            ArrangeGetDepartmentFlowById();

            // Act
            var actual = departmentFlowService.GetDepartmentFlowById(null);

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<FlowModel>), actual);
        }

        [Test]
        public void GetDepartmentFlowById_VaidArg_ReturnExpectedFlows()
        {
            // Arrange
            var citeria = new GetDepartmentFlowByIdCriteria()
            {
                DepartmentId = 1,
                FlowTemplateId = 1
            };
            IEnumerable<FlowModel> expectedFlows = GetListOfFlowForTest();
            ArrangeGetDepartmentFlowById(expectedFlows);

            // Act
            var actual = departmentFlowService.GetDepartmentFlowById(citeria);

            // Assert
            Assert.AreEqual(expectedFlows, actual);
        }

        [Test]
        public void GetDepartmentFlowById_InVaidArg_ReturnEmptyFlows()
        {
            // Arrange
            IEnumerable<FlowModel> expectedFlows = GetListOfFlowForTest();
            ArrangeGetDepartmentFlowById(expectedFlows);

            // Act
            var actual = departmentFlowService.GetDepartmentFlowById(null);

            // Assert
            Assert.IsEmpty(actual);
        }

        #endregion GetDepartmentFlowById

        #region CreateDepartmentFlow

        [Test]
        public void CreateDepartmentFlow_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeExecuteDepartmentFlow();

            // Act
            var actual = departmentFlowService.CreateDepartmentFlow(Arg.Any<CreateDepartmentFlowCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void CreateDepartmentFlow_VaidArg_ExpectedToExecuteInsert()
        {
            // Arrange
            var citeria = new CreateDepartmentFlowCriteria()
            {
                DepartmentId = 1,
                FlowTemplateId = 1,
                ModifiedBy = 1
            };
            ArrangeExecuteDepartmentFlow();

            // Act
            var actual = departmentFlowService.CreateDepartmentFlow(citeria);

            // Assert
            departmentFlowService.Repository.Received().Insert(Arg.Is<CreateDepartmentFlowCriteria>(x => x.IsValid()));
        }

        #endregion CreateDepartmentFlow

        #region DeleteDepartmentFlow

        [Test]
        public void DeleteDepartmentFlow_AnyArg_ReturnInstanceOfCommandResult()
        {
            // Arrange
            ArrangeExecuteDepartmentFlow();

            // Act
            var actual = departmentFlowService.DeleteDepartmentFlow(Arg.Any<DeleteDepartmentFlowCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(CommandResult), actual);
        }

        [Test]
        public void DeleteDepartmentFlow_VaidArg_ExpectedToExecuteDelete()
        {
            // Arrange
            var citeria = new DeleteDepartmentFlowCriteria()
            {
                DepartmentId = 1,
                FlowTemplateId = 1,
                ModifiedBy = 1
            };
            ArrangeExecuteDepartmentFlow();

            // Act
            var actual = departmentFlowService.DeleteDepartmentFlow(citeria);

            // Assert
            departmentFlowService.Repository.Received().Delete(Arg.Is<DeleteDepartmentFlowCriteria>(x => x.IsValid()));
        }

        #endregion DeleteDepartmentFlow

        #region GetDepartmentHasSettingByFlowTemplateId

        [Test]
        public void GetDepartmentHasSettingByFlowTemplateId_AnyArg_ReturnInstanceOfListOfDepartment()
        {
            // Arrange
            ArrangeGetDepartmentHasSettingByFlowTemplateId();

            // Act
            var actual = departmentFlowService.GetDepartmentHasSettingByFlowTemplateId(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(IList<Department>), actual);
        }

        [Test]
        public void GetDepartmentHasSettingByFlowTemplateId_VaidArg_ReturnExpectedDepartments()
        {
            // Arrange
            var expectedDepartments = new List<Department>();
            expectedDepartments.Add(new Department());
            ArrangeGetDepartmentHasSettingByFlowTemplateId(expectedDepartments);

            // Act
            var actual = departmentFlowService.GetDepartmentHasSettingByFlowTemplateId(Arg.Any<int>());

            // Assert
            Assert.AreEqual(expectedDepartments, actual);
        }

        #endregion GetDepartmentHasSettingByFlowTemplateId

        #region Helpers

        private IBaseRepository FakeBaseRepository()
        {
            var repository = Substitute.For<IBaseRepository>();

            return repository;
        }

        private IFlowSettingRepository FakeFlowSettingRepository()
        {
            var repository = Substitute.For<IFlowSettingRepository>();

            return repository;
        }

        private IEnumerable<DepartmentFlowModel> GetListOfDepartmentFlowForTest()
        {
            var flowTemplates = new List<DepartmentFlowModel>
            {
                new DepartmentFlowModel
                {
                    DepartmentId = 1,
                    Code = "HR",
                    Name = "HR",
                    HumanResources = new List<ProcessFlowModel>
                    {
                        new ProcessFlowModel { EmployeeId = 1, FullName = "a" }
                    }
                },
                new DepartmentFlowModel
                {
                    DepartmentId = 2,
                    Code = "Management",
                    Name = "Management"
                }
            };

            return flowTemplates;
        }

        private IEnumerable<FlowModel> GetListOfFlowForTest()
        {
            var flows = new List<FlowModel>
            {
                new FlowModel
                {
                    ProcessId = 1
                }
            };

            return flows;
        }

        private void ArrangeGetDepartmentFlowByFlowTemplateId(IEnumerable<DepartmentFlowModel> departmentFlows = null)
        {
            flowSettingRepository = FakeFlowSettingRepository();

            flowSettingRepository.GetDepartmentFlowByFlowTemplateId(Arg.Any<GetDepartmentFlowByFlowTemplateIdCriteria>())
                .Returns(departmentFlows ?? new List<DepartmentFlowModel>());
            departmentFlowService = new DepartmentFlowService(flowSettingRepository);
        }

        private void ArrangeGetDepartmentFlowById(IEnumerable<FlowModel> departmentFlows = null)
        {
            flowSettingRepository = FakeFlowSettingRepository();

            flowSettingRepository.GetFlowSettingByFlowTemplateId(Arg.Any<GetDepartmentFlowByIdCriteria>())
                .Returns(departmentFlows ?? new List<FlowModel>());
            departmentFlowService = new DepartmentFlowService(flowSettingRepository);
        }

        private void ArrangeExecuteDepartmentFlow(CommandResult result = null)
        {
            var baseRepository = FakeBaseRepository();
            departmentFlowService = new DepartmentFlowService(flowSettingRepository);
            departmentFlowService.Repository = baseRepository;
            departmentFlowService.Repository.Insert(Arg.Any<ICriteria>()).Returns(result ?? new CommandResult());
            departmentFlowService.Repository.Update(Arg.Any<ICriteria>()).Returns(result ?? new CommandResult());
            departmentFlowService.Repository.Delete(Arg.Any<ICriteria>()).Returns(result ?? new CommandResult());
        }

        private void ArrangeGetDepartmentHasSettingByFlowTemplateId(IList<Department> departments = null)
        {
            departmentFlowService = Substitute.For<DepartmentFlowService>();
            departmentFlowService.GetData<Department>(Arg.Any<GetDepartmentSectionHasSettingCriteria>()).Returns(departments ?? new List<Department>());
        }

        #endregion Helpers
    }
}