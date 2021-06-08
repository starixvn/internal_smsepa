namespace WindowsService
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.Data.SqlServer;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Serilog;
    using Serilog.Events;
    using Serilog.Sinks.SystemConsole.Themes;
    using SMS.EPA.WindowsService;
    using SMS.EPA.WindowsService.Services;
    using SMS.EPA.WindowsService.ThirdPartyAPI;
    using System;
    using System.Configuration;
    using System.IO;
    using System.Linq;

    public class Program
    {
        public static void Main(string[] args)
        {
            const string loggerTemplate = @"{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u4}]<{ThreadId}> [{SourceContext:l}] {Message:lj}{NewLine}{Exception}";
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var logfile = Path.Combine(baseDir, "logs", "log.txt");
            Log.Logger = new LoggerConfiguration().MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console(LogEventLevel.Information, loggerTemplate, theme: AnsiConsoleTheme.Literate)
                .WriteTo.File(logfile, LogEventLevel.Information, loggerTemplate, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 90)
                .CreateLogger();

            try
            {
                Log.Information("Starting up the service.");
                CreateHostBuilder(args).Build().Run();
                return;

            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "There was a problem starting the service");
                return;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((context, config) =>
                {
                    var configuration = config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
                    var connections = configuration.GetSection("ConnectionStrings")
                     .GetChildren()
                     .AsEnumerable()
                     .Where(x => !string.IsNullOrWhiteSpace(x.GetValue<string>("Name")))
                     .ToDictionary(x => x.GetValue<string>("Name"),
                    y => new ConnectionConfiguration(y.GetValue<string>("Name"), y.GetValue<string>("ConnectionString"), y.GetValue<string>("ProviderName")));
                    string spSettings = "~/App_Data";

                    DbSettingProviderManager
                       .StartNewSession()
                       .Use(connections)
                       .WithSqlServer()
                       .UseDefaultDbSettingProvider(spSettings)
                       .Run();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IDepartmentService, DepartmentService>();
                    services.AddSingleton<IDisciplineSummaryService, DisciplineSummaryService>();
                    services.AddSingleton<IEmployeeService, EmployeeService>();
                    services.AddSingleton<IJobTitleService, JobTitleService>();
                    services.AddSingleton<ISupervisorService, DisciplinePermissionService>();
                    services.AddSingleton<ITokenService, TokenService>();
                    services.AddSingleton<ISynchronizeService, SynchronizeService>();
                    services.AddSingleton<ITeamService, TeamService>();
                    services.AddSingleton<IPhotoService, PhotoService>();
                    services.AddSingleton<IJobLevelService, JobLevelService>();
                    services.AddSingleton<IDynamicRepository, DynamicRepository>();
                    services.AddSingleton<IDepartmentAPI, DepartmentAPI>();
                    services.AddSingleton<IDisciplineSummaryAPI, DisciplineSummaryAPI>();
                    services.AddSingleton<IEmployeeAPI, EmployeeAPI>();
                    services.AddSingleton<IJobTitleAPI, JobTitleAPI>();
                    services.AddSingleton<IEmployeeSupervisorAPI, EmployeeDisciplinePermissionAPI>();
                    services.AddSingleton<ITeamSupervisorAPI, TeamDisciplinePermissionAPI>();
                    services.AddSingleton<ITeamAPI, TeamAPI>();
                    services.AddSingleton<IPhotoAPI, PhotoAPI>();
                    services.AddSingleton<IJobLevelAPI, JobLevelAPI>();
                    services.AddSingleton<ITokenAPI, TokenAPI>();

                    services.Configure<ApplicationConfig>(hostContext.Configuration.GetSection("AppSettings"));
                    services.AddHostedService<Worker>();
                })
                .UseSerilog();
        }
    }
}
