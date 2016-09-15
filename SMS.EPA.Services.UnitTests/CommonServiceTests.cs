namespace SMS.EPA.Services.UnitTests
{
    using NSubstitute;
    using NUnit.Framework;
    using SMS.EPA.Criteria;
    using SMS.EPA.Criteria.Department;
    using SMS.EPA.Criteria.EmployeeEPA;
    using SMS.EPA.Criteria.EPAHistory;
    using SMS.EPA.Criteria.Period;
    using SMS.EPA.Criteria.TargetType;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;
    using System.Collections.Generic;

    [TestFixture]
    public class CommonServiceTests
    {
        private ITargetTypeService targetTypeService;
        private IKPIService kpiService;
        private IKPIGroupService kpiGroupService;
        private IEmployeeService employeeService;
        private IDepartmentService departmentService;
        private IPeriodService periodService;
        private IEmployeeEPAService employeeEPAService;
        private IEPAHistoryService epaHistoryService;
        private ITargetTemplateService targetTemplateService;
        private IMenuService menuService;
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
            Assert.IsInstanceOf(typeof(IEnumerable<TargetType>), actual);
        }

        [Test]
        public void GetTargetType_VaidArg_ReturnExpectedTargetTypes()
        {
            // Arrange
            var criteria = new GetTargetTypeCriteria();
            var expectedTargetTypes = new List<TargetType>();
            expectedTargetTypes.Add(new TargetType());
            ArrangeGetTargetType(expectedTargetTypes);

            // Act
            var actual = commonService.GetTargetType(criteria);

            // Assert
            Assert.AreEqual(expectedTargetTypes, actual);
        }

        #endregion
        
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
            var expectedKPIs = new List<KPIModel>();
            expectedKPIs.Add(new KPIModel());
            ArrangeGetKPI(expectedKPIs);

            // Act
            var actual = commonService.GetKPI(criteria);

            // Assert
            Assert.AreEqual(expectedKPIs, actual);
        }

        #endregion

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
            var expectedKPIGroups = new List<KPIGroupModel>();
            expectedKPIGroups.Add(new KPIGroupModel());
            ArrangeGetKPIGroup(expectedKPIGroups);

            // Act
            var actual = commonService.GetKPIGroup(criteria);

            // Assert
            Assert.AreEqual(expectedKPIGroups, actual);
        }

        #endregion

        #region GetAllEmployee
        [Test]
        public void GetAllEmployee_AnyArg_ReturnInstanceOfListOfEmployeeModel()
        {
            // Arrange
            commonService = Substitute.For<ICommonService>();

            // Act
            var actual = commonService.GetAllEmployee(Arg.Any<GetAllEmployeeCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<Employee>), actual);
        }

        [Test]
        public void GetAllEmployee_VaidArg_ReturnExpectedEmployees()
        {
            // Arrange
            var criteria = new GetAllEmployeeCriteria();
            var expectedEmployees = new List<Employee>();
            expectedEmployees.Add(new Employee());
            ArrangeGetAllEmployee(expectedEmployees);

            // Act
            var actual = commonService.GetAllEmployee(criteria);

            // Assert
            Assert.AreEqual(expectedEmployees, actual);
        }

        #endregion

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
            var expectedEmployees = new List<Employee>();
            expectedEmployees.Add(new Employee());
            ArrangeGetEmployeeByDepartmentId(expectedEmployees);

            // Act
            var actual = commonService.GetEmployeeByDepartmentId(Arg.Any<int>());

            // Assert
            Assert.AreEqual(expectedEmployees, actual);
        }

        #endregion

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
            var expectedEmployees = new List<Employee>();
            expectedEmployees.Add(new Employee());
            ArrangeGetEmployeeByGroupId(expectedEmployees);

            // Act
            var actual = commonService.GetEmployeeByGroupId(Arg.Any<int>());

            // Assert
            Assert.AreEqual(expectedEmployees, actual);
        }

        #endregion

        #region GetEmployeeById
        
        [Test]
        public void GetEmployeeById_VaidArg_ReturnExpectedEmployee()
        {
            // Arrange
            var expectedEmployee = new Employee();
            ArrangeGetEmployeeById(expectedEmployee);

            // Act
            var actual = commonService.GetEmployeeById(Arg.Any<int>());

            // Assert
            Assert.AreEqual(expectedEmployee, actual);
        }

        #endregion

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
            var expectedGroups = new List<Group>();
            expectedGroups.Add(new Group());
            var commonService = ArrangeGetAllGroup(expectedGroups);

            // Act
            var actual = commonService.GetAllGroup(criteria);

            // Assert
            Assert.AreEqual(expectedGroups, actual);
        }

        #endregion

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
            var department1 = new Department() { ParentId = 1 };
            var department2 = new Department() { ParentId = null };
            departments.Add(department1);
            departments.Add(department2);

            var expectedDepartment = new DepartmentInfoModel()
            {
                Departments = departments,
                TreeDepartment = department2
            };

            ArrangeGetDepartment(departments);

            // Act
            var actual = commonService.GetDepartment(Arg.Any<GetDepartmentCriteria>());

            // Assert
            Assert.AreEqual(expectedDepartment.Departments, departments);
            Assert.AreEqual(expectedDepartment.TreeDepartment, department2);
        }

        [Test]
        public void GetDepartment_EmptyListDepartment_ReturnDefaultDepartment()
        {
            // Arrange
            var departments = new List<Department>();

            var expectedDepartment = new DepartmentInfoModel()
            {
                Departments = departments,
                TreeDepartment = null
            };

            ArrangeGetDepartment(departments);

            // Act
            var actual = commonService.GetDepartment(Arg.Any<GetDepartmentCriteria>());

            // Assert
            Assert.AreEqual(expectedDepartment.Departments, departments);
            Assert.IsNull(expectedDepartment.TreeDepartment);
        }

        #endregion

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
            var expectedPeriods = new List<PeriodModel>();
            expectedPeriods.Add(new PeriodModel());
            ArrangeGetPeriod(expectedPeriods);

            // Act
            var actual = commonService.GetPeriod(Arg.Any<GetPeriodCriteria>());

            // Assert
            Assert.AreEqual(expectedPeriods, actual);
        }

        #endregion

        #region GetStandardGrade
        [Test]
        public void GetStandardGrade_VaidArg_ReturnInstanceOfListOfStandardGradeModel()
        {
            // Arrange
            commonService = Substitute.For<ICommonService>();

            // Act
            var actual = commonService.GetStandardGrade(Arg.Any<GetStandardGradeCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<StandardGradeModel>), actual);
        }

        [Test]
        public void GetStandardGrade_VaidArg_ReturnExpectedStandardGrades()
        {
            // Arrange
            var expectedGrades = new List<StandardGradeModel>();
            expectedGrades.Add(new StandardGradeModel());
            ArrangeGetStandardGrade(expectedGrades);

            // Act
            var actual = commonService.GetStandardGrade(Arg.Any<GetStandardGradeCriteria>());

            // Assert
            Assert.AreEqual(expectedGrades, actual);
        }

        #endregion

        #region GetAllPeriodIncludeImport
        [Test]
        public void GetAllPeriodIncludeImport_AnyArg_ReturnInstanceOfListOfPeriodModel()
        {
            // Arrange
            commonService = Substitute.For<ICommonService>();

            // Act
            var actual = commonService.GetAllPeriodIncludeImport(Arg.Any<GetAllPeriodIncludeImportCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(IList<PeriodModel>), actual);
        }

        [Test]
        public void GetAllPeriodIncludeImport_VaidArg_ReturnExpectedPeriods()
        {
            // Arrange
            var expectedPeriods = new List<PeriodModel>();
            expectedPeriods.Add(new PeriodModel());
            ArrangeGetAllPeriodIncludeImport(expectedPeriods);

            // Act
            var actual = commonService.GetAllPeriodIncludeImport(Arg.Any<GetAllPeriodIncludeImportCriteria>());

            // Assert
            Assert.AreEqual(expectedPeriods, actual);
        }

        #endregion

        #region GetOtherViolationDetailByEPAId
        [Test]
        public void GetOtherViolationDetailByEPAId_AnyArg_ReturnInstanceOfListOfEmployeeOtherViolationDetailModel()
        {
            // Arrange
            commonService = Substitute.For<ICommonService>();

            // Act
            var actual = commonService.GetOtherViolationDetailByEPAId(Arg.Any<GetOtherViolationDetailByEPAIdCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<EmployeeOtherViolationDetailModel>), actual);
        }

        [Test]
        public void GetOtherViolationDetailByEPAId_VaidArg_ReturnExpectedOtherViolations()
        {
            // Arrange
            var otherViolations = new List<EmployeeOtherViolationDetailModel>();
            otherViolations.Add(new EmployeeOtherViolationDetailModel());
            ArrangeGetOtherViolationDetailByEPAId(otherViolations);

            // Act
            var actual = commonService.GetOtherViolationDetailByEPAId(Arg.Any<GetOtherViolationDetailByEPAIdCriteria>());

            // Assert
            Assert.AreEqual(otherViolations, actual);
        }

        #endregion

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
            var targetTemplates = new List<TargetTemplate>();
            targetTemplates.Add(new TargetTemplate());
            ArrangeGetAllTargetTemplate(targetTemplates);

            // Act
            var actual = commonService.GetAllTargetTemplate(Arg.Any<GetAllTargetTemplateCriteria>());

            // Assert
            Assert.AreEqual(targetTemplates, actual);
        }

        #endregion

        #region GetMenusByEmployeeId
        [Test]
        public void GetMenusByEmployeeId_AnyArg_ReturnInstanceOfListOfEmployeeModel()
        {
            // Arrange
            commonService = Substitute.For<ICommonService>();

            // Act
            var actual = commonService.GetMenusByEmployeeId(Arg.Any<int>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<MenuGroupModel>), actual);
        }

        [Test]
        public void GetMenusByEmployeeId_VaidArg_ReturnExpectedMenuGroups()
        {
            // Arrange
            var expectedMenuGroups = new List<MenuGroupModel>();
            expectedMenuGroups.Add(new MenuGroupModel());
            ArrangeGetMenusByEmployeeId(expectedMenuGroups);

            // Act
            var actual = commonService.GetMenusByEmployeeId(Arg.Any<int>());

            // Assert
            Assert.AreEqual(expectedMenuGroups, actual);
        }

        #endregion

        #region GetAllJobTitle
        [Test]
        public void GetAllJobTitle_AnyArg_ReturnInstanceOfListOfTargetTemplateModel()
        {
            // Arrange
            commonService = Substitute.For<ICommonService>();

            // Act
            var actual = commonService.GetAllJobTitle(Arg.Any<GetAllJobTitleCriteria>());

            // Assert
            Assert.IsInstanceOf(typeof(IEnumerable<JobTitle>), actual);
        }

        [Test]
        public void GetAllJobTitle_VaidArg_ReturnExpectedJobTitles()
        {
            // Arrange
            var criteria = new GetAllJobTitleCriteria();
            var expectedJobTitles = new List<JobTitle>();
            expectedJobTitles.Add(new JobTitle());
            var commonService = ArrangeGetAllJobTitle(expectedJobTitles);

            // Act
            var actual = commonService.GetAllJobTitle(criteria);

            // Assert
            Assert.AreEqual(expectedJobTitles, actual);
        }

        #endregion

        private void ArrangeCommon()
        {
            targetTypeService = Substitute.For<ITargetTypeService>();
            kpiService = Substitute.For<IKPIService>();
            kpiGroupService = Substitute.For<IKPIGroupService>();
            employeeService = Substitute.For<IEmployeeService>();
            departmentService = Substitute.For<IDepartmentService>();
            periodService = Substitute.For<IPeriodService>();
            employeeEPAService = Substitute.For<IEmployeeEPAService>();
            epaHistoryService = Substitute.For<IEPAHistoryService>();
            targetTemplateService = Substitute.For<ITargetTemplateService>();
            menuService = Substitute.For<IMenuService>();
            commonService = new CommonService(
                targetTypeService,
                kpiService,
                kpiGroupService,
                employeeService,
                departmentService,
                periodService,
                employeeEPAService,
                epaHistoryService,
                targetTemplateService,
                menuService);
        }

        private void ArrangeGetTargetType(IEnumerable<TargetType> targetTypes = null)
        {
            ArrangeCommon();
            targetTypeService.GetTargetType(Arg.Any<GetTargetTypeCriteria>()).Returns(targetTypes);
        }

        private void ArrangeGetKPI(IEnumerable<KPIModel> kpis = null)
        {
            ArrangeCommon();
            kpiService.GetKPI(Arg.Any<GetKPICriteria>()).Returns(kpis ?? new List<KPIModel>());
        }

        private void ArrangeGetKPIGroup(IEnumerable<KPIGroupModel> kpiGroups = null)
        {
            ArrangeCommon();
            kpiGroupService.GetKPIGroup(Arg.Any<GetKPIGroupCriteria>()).Returns(kpiGroups ?? new List<KPIGroupModel>());
        }

        private void ArrangeGetAllEmployee(IEnumerable<Employee> employees = null)
        {
            ArrangeCommon();
            employeeService.GetAllEmployee(Arg.Any<GetAllEmployeeCriteria>()).Returns(employees ?? new List<Employee>());
        }

        private void ArrangeGetEmployeeByDepartmentId(IEnumerable<Employee> employees = null)
        {
            ArrangeCommon();
            employeeService.GetEmployeeByDepartmentId(Arg.Any<int>()).Returns(employees ?? new List<Employee>());
        }

        private void ArrangeGetEmployeeByGroupId(IEnumerable<Employee> employees = null)
        {
            ArrangeCommon();
            employeeService.GetEmployeeByGroupId(Arg.Any<int>()).Returns(employees ?? new List<Employee>());
        }

        private void ArrangeGetEmployeeById(Employee employee = null)
        {
            ArrangeCommon();
            employeeService.GetEmployeeById(Arg.Any<int>()).Returns(employee ?? new Employee());
        }

        private CommonService ArrangeGetAllGroup(IEnumerable<Group> groups = null)
        {
            var commonService = Substitute.For<CommonService>();
            commonService.GetData<Group>(Arg.Any<GetAllGroupCriteria>()).Returns(groups ?? new List<Group>());
            return commonService;
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
            employeeEPAService.GetStandardGrade(Arg.Any<GetStandardGradeCriteria>()).Returns(grades ?? new List<StandardGradeModel>());
        }

        private void ArrangeGetAllPeriodIncludeImport(IList<PeriodModel> periods = null)
        {
            ArrangeCommon();
            epaHistoryService.GetAllPeriodIncludeImport(Arg.Any<GetAllPeriodIncludeImportCriteria>()).Returns(periods ?? new List<PeriodModel>());
        }

        private void ArrangeGetOtherViolationDetailByEPAId(IEnumerable<EmployeeOtherViolationDetailModel> otherViolations = null)
        {
            ArrangeCommon();
            employeeEPAService.GetOtherViolationDetailByEPAId(Arg.Any<GetOtherViolationDetailByEPAIdCriteria>())
                .Returns(otherViolations ?? new List<EmployeeOtherViolationDetailModel>());
        }

        private void ArrangeGetAllTargetTemplate(IEnumerable<TargetTemplate> targetTemplates = null)
        {
            ArrangeCommon();
            targetTemplateService.GetAllTargetTemplate(Arg.Any<GetAllTargetTemplateCriteria>()).Returns(targetTemplates ?? new List<TargetTemplate>());
        }

        private void ArrangeGetMenusByEmployeeId(IEnumerable<MenuGroupModel> menuGroups = null)
        {
            ArrangeCommon();
            menuService.GetMenusByEmployeeId(Arg.Any<int>()).Returns(menuGroups ?? new List<MenuGroupModel>());
        }

        private CommonService ArrangeGetAllJobTitle(IEnumerable<JobTitle> jobTitles = null)
        {
            var commonService = Substitute.For<CommonService>();
            commonService.GetData<JobTitle>(Arg.Any<GetAllJobTitleCriteria>()).Returns(jobTitles ?? new List<JobTitle>());
            return commonService;
        }
    }
}
