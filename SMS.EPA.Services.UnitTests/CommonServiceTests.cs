namespace SMS.EPA.Services.UnitTests
{
    using System.Collections.Generic;
    using Core.Services;
    using Criteria;
    using Criteria.EPAHistory;
    using Criteria.TargetType;
    using HRM.UserControl.Client.Core.Models.Role;
    using Model;
    using Model.Enitites;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class CommonServiceTests
    {
        private IKPIService kpiService;
        private IBaseService baseService;
        private IEmployeeService employeeService;
        private IDepartmentService departmentService;
        private IPeriodService periodService;
        private ICommonService commonService;

        #region GetTargetType

        [Test]
        public void GetTargetType_AnyArg_ReturnInstanceOfListOfTargetTypeModel()
        {
            // Arrange
            commonService = Substitute.For<ICommonService>();

            // Act
            var actual = commonService.GetTargetType(Arg.Any<GetTargetTypeCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<TargetTypeModel>), actual);
        }

        [Test]
        public void GetTargetType_ValidArg_ReturnExpectedTargetTypes()
        {
            // Arrange
            var criteria = new GetTargetTypeCriteria();
            var expectedTargetTypes = new List<TargetTypeModel> { new TargetTypeModel() };
            ArrangeGetTargetType(expectedTargetTypes);

            // Act
            var actual = commonService.GetTargetType(criteria);

            // Assert
            Assert.AreEqual(expectedTargetTypes, actual);
        }

        #endregion GetTargetType

        #region GetKPI

        [Test]
        public void GetKPI_AnyArg_ReturnInstanceOfListOfKPIModel()
        {
            // Arrange
            commonService = Substitute.For<ICommonService>();

            // Act
            var actual = commonService.GetKPI(Arg.Any<GetKPICriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<KPIModel>), actual);
        }

        [Test]
        public void GetKPI_VaidArg_ReturnExpectedKPIs()
        {
            // Arrange
            var criteria = new GetKPICriteria();
            var expectedKPIs = new List<KPIModel> { new KPIModel() };
            ArrangeGetKPI(expectedKPIs);

            // Act
            var actual = commonService.GetKPI(criteria);

            // Assert
            Assert.AreEqual(expectedKPIs, actual);
        }

        #endregion GetKPI

        #region GetKPIGroup

        [Test]
        public void GetKPIGroup_AnyArg_ReturnInstanceOfListOfKPIGroupModel()
        {
            // Arrange
            commonService = Substitute.For<ICommonService>();

            // Act
            var actual = commonService.GetKPIGroup(Arg.Any<GetKPIGroupCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<KPIGroupModel>), actual);
        }

        [Test]
        public void GetKPIGroup_VaidArg_ReturnExpectedKPIGroups()
        {
            // Arrange
            var criteria = new GetKPIGroupCriteria();
            var expectedKPIGroups = new List<KPIGroupModel> { new KPIGroupModel() };
            ArrangeGetKPIGroup(expectedKPIGroups);

            // Act
            var actual = commonService.GetKPIGroup(criteria);

            // Assert
            Assert.AreEqual(expectedKPIGroups, actual);
        }

        #endregion GetKPIGroup

        #region GetEmployeeByDepartmentId

        [Test]
        public void GetEmployeeByDepartmentId_AnyArg_ReturnInstanceOfListOfEmployeeModel()
        {
            // Arrange
            commonService = Substitute.For<ICommonService>();

            // Act
            var actual = commonService.GetEmployeeByDepartmentId(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<Employee>), actual);
        }

        [Test]
        public void GetEmployeeByDepartmentId_VaidArg_ReturnExpectedEmployees()
        {
            // Arrange
            var expectedEmployees = new List<Employee> { new Employee() };
            ArrangeGetEmployeeByDepartmentId(expectedEmployees);

            // Act
            var actual = commonService.GetEmployeeByDepartmentId(Arg.Any<int>());

            // Assert
            Assert.AreEqual(expectedEmployees, actual);
        }

        #endregion GetEmployeeByDepartmentId

        #region GetEmployeeByGroupId

        [Test]
        public void GetEmployeeByGroupId_AnyArg_ReturnInstanceOfListOfEmployeeModel()
        {
            // Arrange
            commonService = Substitute.For<ICommonService>();

            // Act
            var actual = commonService.GetEmployeeByGroupId(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<Employee>), actual);
        }

        [Test]
        public void GetEmployeeByGroupId_VaidArg_ReturnExpectedEmployees()
        {
            // Arrange
            var expectedEmployees = new List<Employee> { new Employee() };
            ArrangeGetEmployeeByGroupId(expectedEmployees);

            // Act
            var actual = commonService.GetEmployeeByGroupId(Arg.Any<int>());

            // Assert
            Assert.AreEqual(expectedEmployees, actual);
        }

        #endregion GetEmployeeByGroupId

        #region GetEmployeeById

        [Test]
        public void GetEmployeeById_VaidArg_ReturnExpectedEmployee()
        {
            // Arrange
            var expectedEmployee = new Employee();
            ArrangeCommon();
            employeeService.GetEmployeeById(Arg.Any<int>(), Arg.Any<IList<Role>>()).Returns(expectedEmployee);

            // Act
            var actual = commonService.GetEmployeeById(Arg.Any<int>(), Arg.Any<IList<Role>>());

            // Assert
            Assert.AreEqual(expectedEmployee, actual);
        }

        #endregion GetEmployeeById

        #region GetAllGroup

        [Test]
        public void GetAllGroup_AnyArg_ReturnInstanceOfListOfGroup()
        {
            // Arrange
            commonService = Substitute.For<ICommonService>();

            // Act
            var actual = commonService.GetAllGroup(Arg.Any<GetAllGroupCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<Group>), actual);
        }

        [Test]
        public void GetAllGroup_VaidArg_ReturnExpectedGroups()
        {
            // Arrange
            var criteria = new GetAllGroupCriteria();
            var expectedGroups = new List<Group> { new Group() };
            ArrangeGetAllGroup(expectedGroups);

            // Act
            var actual = commonService.GetAllGroup(criteria);

            // Assert
            Assert.AreEqual(expectedGroups, actual);
        }

        #endregion GetAllGroup

        #region GetDepartment

        [Test]
        public void GetDepartment_AnyArg_ReturnInstanceOfDepartmentInfoModel()
        {
            // Arrange
            ArrangeGetDepartment();

            // Act
            var actual = commonService.GetDepartment(Arg.Any<GetDepartmentCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(DepartmentInfoModel), actual);
        }

        [Test]
        public void GetDepartment_NotEmptyListDepartment_ReturnExpectedDepartment()
        {
            // Arrange
            var departments = new List<Department>();
            var department1 = new Department { ParentId = 1 };
            var department2 = new Department { ParentId = null };
            departments.Add(department1);
            departments.Add(department2);

            var expectedDepartment = new DepartmentInfoModel
            {
                Departments = departments,
                TreeDepartment = department2
            };

            ArrangeGetDepartment(departments);

            // Act
            commonService.GetDepartment(Arg.Any<GetDepartmentCriteria>());

            // Assert
            Assert.AreEqual(expectedDepartment.Departments, departments);
            Assert.AreEqual(expectedDepartment.TreeDepartment, department2);
        }

        [Test]
        public void GetDepartment_EmptyListDepartment_ReturnDefaultDepartment()
        {
            // Arrange
            var departments = new List<Department>();

            var expectedDepartment = new DepartmentInfoModel
            {
                Departments = departments,
                TreeDepartment = null
            };

            ArrangeGetDepartment(departments);

            // Act
            commonService.GetDepartment(Arg.Any<GetDepartmentCriteria>());

            // Assert
            Assert.AreEqual(expectedDepartment.Departments, departments);
            Assert.IsNull(expectedDepartment.TreeDepartment);
        }

        #endregion GetDepartment

        #region GetPeriod

        [Test]
        public void GetPeriod_VaidArg_ReturnInstanceOfListOfPeriodModel()
        {
            // Arrange
            commonService = Substitute.For<ICommonService>();

            // Act
            var actual = commonService.GetPeriod(Arg.Any<GetPeriodCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<PeriodModel>), actual);
        }

        [Test]
        public void GetPeriod_VaidArg_ReturnExpectedPeriods()
        {
            // Arrange
            var expectedPeriods = new List<PeriodModel> { new PeriodModel() };
            ArrangeGetPeriod(expectedPeriods);

            // Act
            var actual = commonService.GetPeriod(Arg.Any<GetPeriodCriteria>());

            // Assert
            Assert.AreEqual(expectedPeriods, actual);
        }

        #endregion GetPeriod

        #region GetStandardGrade

        [Test]
        public void GetStandardGrade_VaidArg_ReturnInstanceOfListOfStandardGradeModel()
        {
            // Arrange
            commonService = Substitute.For<ICommonService>();

            // Act
            var actual = commonService.GetStandardGrade();

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<StandardGradeModel>), actual);
        }

        [Test]
        public void GetStandardGrade_VaidArg_ReturnExpectedStandardGrades()
        {
            // Arrange
            var expectedGrades = new List<StandardGradeModel> { new StandardGradeModel() };
            ArrangeGetStandardGrade(expectedGrades);

            // Act
            var actual = commonService.GetStandardGrade();

            // Assert
            Assert.AreEqual(expectedGrades, actual);
        }

        #endregion GetStandardGrade

        #region GetAllPeriodIncludeImport

        [Test]
        public void GetAllPeriodIncludeImport_AnyArg_ReturnInstanceOfListOfPeriodModel()
        {
            // Arrange
            commonService = Substitute.For<ICommonService>();

            // Act
            var actual = commonService.GetAllPeriodIncludeImport(Arg.Any<GetAllPeriodIncludeImportCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<PeriodModel>), actual);
        }

        [Test]
        public void GetAllPeriodIncludeImport_VaidArg_ReturnExpectedPeriods()
        {
            // Arrange
            var expectedPeriods = new List<PeriodModel> { new PeriodModel() };
            ArrangeGetAllPeriodIncludeImport(expectedPeriods);

            // Act
            var actual = commonService.GetAllPeriodIncludeImport(Arg.Any<GetAllPeriodIncludeImportCriteria>());

            // Assert
            Assert.AreEqual(expectedPeriods, actual);
        }

        #endregion GetAllPeriodIncludeImport

        #region GetAllTargetTemplate

        [Test]
        public void GetAllTargetTemplate_AnyArg_ReturnInstanceOfListOfTargetTemplateModel()
        {
            // Arrange
            commonService = Substitute.For<ICommonService>();

            // Act
            var actual = commonService.GetAllTargetTemplate(Arg.Any<GetAllTargetTemplateCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<TargetTemplate>), actual);
        }

        [Test]
        public void GetAllTargetTemplate_VaidArg_ReturnExpectedTargetTemplates()
        {
            // Arrange
            var targetTemplates = new List<TargetTemplate> { new TargetTemplate() };
            ArrangeGetAllTargetTemplate(targetTemplates);

            // Act
            var actual = commonService.GetAllTargetTemplate(Arg.Any<GetAllTargetTemplateCriteria>());

            // Assert
            Assert.AreEqual(targetTemplates, actual);
        }

        #endregion GetAllTargetTemplate

        #region GetAllJobTitle

        [Test]
        public void GetAllJobTitle_AnyArg_ReturnInstanceOfListOfTargetTemplateModel()
        {
            // Arrange
            commonService = Substitute.For<ICommonService>();

            // Act
            var actual = commonService.GetAllJobTitle();

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<JobTitle>), actual);
        }

        [Test]
        public void GetAllJobTitle_VaidArg_ReturnExpectedJobTitles()
        {
            // Arrange
            var expectedJobTitles = new List<JobTitle> { new JobTitle() };
            ArrangeGetAllJobTitle(expectedJobTitles);

            // Act
            var actual = commonService.GetAllJobTitle();

            // Assert
            Assert.AreEqual(expectedJobTitles, actual);
        }

        #endregion GetAllJobTitle

        [Test]
        public void GetScoreRatingType_AnyArg_ReturnExpectedRatingTypes()
        {
            // Arrange
            var expectedRatingTypes = new List<ScoreRatingTypeModel> { new ScoreRatingTypeModel() };
            ArrangeGetScoreRatingType(expectedRatingTypes);

            // Act
            var actual = commonService.GetScoreRatingType(Arg.Any<GetScoreRatingTypeCriteria>());

            // Assert
            Assert.AreEqual(expectedRatingTypes, actual);
        }

        #region Helpers

        private void ArrangeCommon()
        {
            kpiService = Substitute.For<IKPIService>();
            baseService = Substitute.For<IBaseService>();
            employeeService = Substitute.For<IEmployeeService>();
            departmentService = Substitute.For<IDepartmentService>();
            periodService = Substitute.For<IPeriodService>();

            commonService = new CommonService(
                kpiService,
                baseService,
                employeeService,
                departmentService,
                periodService);
        }

        private void ArrangeGetTargetType(IEnumerable<TargetTypeModel> targetTypes = null)
        {
            ArrangeCommon();
            baseService.GetData<TargetTypeModel>(Arg.Any<GetTargetTypeCriteria>()).Returns(targetTypes ?? new List<TargetTypeModel>());
        }

        private void ArrangeGetKPI(IEnumerable<KPIModel> kpis = null)
        {
            ArrangeCommon();
            kpiService.GetKPI(Arg.Any<GetKPICriteria>()).Returns(kpis ?? new List<KPIModel>());
        }

        private void ArrangeGetKPIGroup(IEnumerable<KPIGroupModel> kpiGroups = null)
        {
            ArrangeCommon();
            baseService.GetData<KPIGroupModel>(Arg.Any<GetKPIGroupCriteria>()).Returns(kpiGroups ?? new List<KPIGroupModel>());
        }

        private void ArrangeGetEmployeeByDepartmentId(IEnumerable<Employee> employees = null)
        {
            ArrangeCommon();
            baseService.GetData<Employee>(Arg.Any<GetEmployeeByDepartmentIdCriteria>()).Returns(employees ?? new List<Employee>());
        }

        private void ArrangeGetEmployeeByGroupId(IEnumerable<Employee> employees = null)
        {
            ArrangeCommon();
            baseService.GetData<Employee>(Arg.Any<GetEmployeeByGroupIdCriteria>()).Returns(employees ?? new List<Employee>());
        }

        private void ArrangeGetAllGroup(IEnumerable<Group> groups = null)
        {
            ArrangeCommon();
            baseService.GetData<Group>(Arg.Any<GetAllGroupCriteria>()).Returns(groups ?? new List<Group>());
        }

        private void ArrangeGetDepartment(IList<Department> departments = null)
        {
            ArrangeCommon();
            departmentService.GetDepartment(Arg.Any<GetDepartmentCriteria>()).Returns(departments ?? new List<Department>());
        }

        private void ArrangeGetPeriod(IEnumerable<PeriodModel> periods = null)
        {
            ArrangeCommon();
            periodService.GetPeriod(Arg.Any<GetPeriodCriteria>()).Returns(periods ?? new List<PeriodModel>());
        }

        private void ArrangeGetStandardGrade(IEnumerable<StandardGradeModel> grades = null)
        {
            ArrangeCommon();
            baseService.GetData<StandardGradeModel>(Arg.Any<GetStandardGradeCriteria>()).Returns(grades ?? new List<StandardGradeModel>());
        }

        private void ArrangeGetAllPeriodIncludeImport(IEnumerable<PeriodModel> periods = null)
        {
            ArrangeCommon();
            baseService.GetData<PeriodModel>(Arg.Any<GetAllPeriodIncludeImportCriteria>()).Returns(periods ?? new List<PeriodModel>());
        }

        private void ArrangeGetAllTargetTemplate(IEnumerable<TargetTemplate> targetTemplates = null)
        {
            ArrangeCommon();
            baseService.GetData<TargetTemplate>(Arg.Any<GetAllTargetTemplateCriteria>()).Returns(targetTemplates ?? new List<TargetTemplate>());
        }

        private void ArrangeGetAllJobTitle(IEnumerable<JobTitle> jobTitles = null)
        {
            ArrangeCommon();
            baseService.GetData<JobTitle>(Arg.Any<GetAllJobTitleCriteria>()).Returns(jobTitles ?? new List<JobTitle>());
        }

        private void ArrangeGetScoreRatingType(IEnumerable<ScoreRatingTypeModel> ratingTypes = null)
        {
            ArrangeCommon();
            baseService.GetData<ScoreRatingTypeModel>(Arg.Any<GetScoreRatingTypeCriteria>()).Returns(ratingTypes ?? new List<ScoreRatingTypeModel>());
        }

        #endregion Helpers
    }
}