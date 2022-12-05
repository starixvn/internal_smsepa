namespace SMS.EPA.AutoJob
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.SqlServer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using SMS.EPA.AutoJob.Models;
    using SMS.EPA.AutoJob.Service;
    using SMS.EPA.AutoJob.Workers;
    using SMS.EPA.Core.Repositories;
    using SMS.EPA.Core.Services;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            AddConfiguration(services, Configuration);
            AddDependencyInjection(services);
            AddWorker(services);
        }

        private static void AddConfiguration(IServiceCollection services, IConfiguration configuration)
        {
            var connections = configuration.GetSection("ConnectionStrings")
                                    .GetChildren()
                                    .AsEnumerable()
                                    .Where(x => !string.IsNullOrWhiteSpace(x.GetValue<string>("Name")))
                                    .ToDictionary(x => x.GetValue<string>("Name"),
                                    y => new ConnectionConfiguration(y.GetValue<string>("Name"), y.GetValue<string>("ConnectionString"), y.GetValue<string>("ProviderName")));

            DbSettingProviderManager
                       .StartNewSession()
                       .Use(connections)
                       .WithSqlServer()
                       .UseDefaultDbSettingProvider("~/App_Data/SpNames.xml")
                       .Run();

            services.Configure<IConfiguration>(configuration);
            services.AddSingleton(configuration);
            services.Configure<List<JobSetting>>(configuration.GetSection("JobSettings"));
            services.Configure<UCSettings>(configuration.GetSection("UCSettings"));
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static void AddDependencyInjection(IServiceCollection services)
        {
            services.AddSingleton<IBaseService, BaseService>();
            services.AddSingleton<IBaseRepository, BaseRepository>();
            services.AddSingleton<IEPAUserMappingService, EPAUserMappingService>();
        }

        private static void AddWorker(IServiceCollection services)
        {
            services.AddHostedService<EPAUserMappingWorker>();
        }
    }
}
