namespace SMS.EPA.ClientHRM
{
    using Fanex.Data.Repository;
    using SimpleInjector;
    using SMS.EPA.ClientHRM.Services;
    using SMS.EPA.ClientHRM.ThirdPartyAPIs;

    public class Bootstrap
    {
        private static Container _container;

        public static Container Container
        {
            get
            {
                return _container;
            }
        }

        public static void Start()
        {
            _container = new Container();
            _container.Register<IDepartmentService, DepartmentService>(Lifestyle.Singleton);
            _container.Register<IAttendanceService, AttendanceService>(Lifestyle.Singleton);
            _container.Register<IEmployeeService, EmployeeService>(Lifestyle.Singleton);
            _container.Register<IJobTitleService, JobTitleService>(Lifestyle.Singleton);
            _container.Register<ISupervisorService, SupervisorService>(Lifestyle.Singleton);
            _container.Register<ITokenService, TokenService>(Lifestyle.Singleton);
            _container.Register<ISynchronizeService, SynchronizeService>(Lifestyle.Singleton);
            _container.Register<ITeamService, TeamService>(Lifestyle.Singleton);

            _container.Register<IDynamicRepository, DynamicRepository>(Lifestyle.Singleton);

            _container.Register<IDepartmentAPI, DepartmentAPI>(Lifestyle.Singleton);
            _container.Register<IAttendanceAPI, AttendanceAPI>(Lifestyle.Singleton);
            _container.Register<IEmployeeAPI, EmployeeAPI>(Lifestyle.Singleton);
            _container.Register<IJobTitleAPI, JobTitleAPI>(Lifestyle.Singleton);
            _container.Register<IEmployeeSupervisorAPI, EmployeeDisciplinePermissionAPI>(Lifestyle.Singleton);
            _container.Register<ITeamSupervisorAPI, TeamDisciplinePermissionAPI>(Lifestyle.Singleton);
            _container.Register<ITeamAPI, TeamAPI>(Lifestyle.Singleton);
            _container.Register<ITokenAPI, TokenAPI>(Lifestyle.Singleton);

            _container.Verify();
        }
    }
}
