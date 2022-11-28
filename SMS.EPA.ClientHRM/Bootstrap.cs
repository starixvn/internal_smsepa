namespace SMS.EPA.ClientHrm
{
    using System.Diagnostics.CodeAnalysis;
    using Fanex.Data.Repository;
    using Services;
    using SimpleInjector;
    using SMS.EPA.Core.Repositories;
    using SMS.EPA.Core.Services;
    using ThirdPartyAPI;

    public static class Bootstrap
    {
        public static Container Container { get; set; }

        public static void Start()
        {
            Container = new Container();

            Container.Register<IBaseRepository, BaseRepository>(Lifestyle.Singleton);
            Container.Register<IBaseService, BaseService>(Lifestyle.Singleton);
            Container.Register<IDepartmentService, DepartmentService>(Lifestyle.Singleton);
            Container.Register<IJobLevelService, JobLevelService>(Lifestyle.Singleton);
            Container.Register<IDisciplineSummaryService, DisciplineSummaryService>(Lifestyle.Singleton);
            Container.Register<IEmployeeService, EmployeeService>(Lifestyle.Singleton);
            Container.Register<IJobTitleService, JobTitleService>(Lifestyle.Singleton);
            Container.Register<ISupervisorService, DisciplinePermissionService>(Lifestyle.Singleton);
            Container.Register<ITokenService, TokenService>(Lifestyle.Singleton);
            Container.Register<ISynchronizeService, SynchronizeService>(Lifestyle.Singleton);
            Container.Register<ITeamService, TeamService>(Lifestyle.Singleton);
            Container.Register<IPhotoService, PhotoService>(Lifestyle.Singleton);
            Container.Register<IUserMappingService, UserMappingService>(Lifestyle.Singleton);
            Container.Register<IDynamicRepository, DynamicRepository>(Lifestyle.Singleton);
            Container.Register<IDepartmentAPI, DepartmentAPI>(Lifestyle.Singleton);
            Container.Register<IDisciplineSummaryAPI, DisciplineSummaryAPI>(Lifestyle.Singleton);
            Container.Register<IEmployeeAPI, EmployeeAPI>(Lifestyle.Singleton);
            Container.Register<IJobTitleAPI, JobTitleAPI>(Lifestyle.Singleton);
            Container.Register<IEmployeeSupervisorAPI, EmployeeDisciplinePermissionAPI>(Lifestyle.Singleton);
            Container.Register<ITeamSupervisorAPI, TeamDisciplinePermissionAPI>(Lifestyle.Singleton);
            Container.Register<ITeamAPI, TeamAPI>(Lifestyle.Singleton);
            Container.Register<IPhotoAPI, PhotoAPI>(Lifestyle.Singleton);
            Container.Register<ITokenAPI, TokenAPI>(Lifestyle.Singleton);
            Container.Register<IJobLevelAPI, JobLevelAPI>(Lifestyle.Singleton);

            Container.Verify();
        }
    }
}