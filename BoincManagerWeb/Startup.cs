using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BoincManager;
using BoincManager.Models;
using BoincManagerWeb.Hubs;
using System.IO;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using BoincManagerWeb.Helpers;

namespace BoincManagerWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // From:
            // - https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview?view=aspnetcore-3.0
            // - https://github.com/dotnet/dotnet-docker/blob/master/samples/aspnetapp/aspnetcore-docker-https.md
            bool inDocker = string.Equals(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), "true", StringComparison.OrdinalIgnoreCase);
            if (inDocker && !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Path")))
            {
                services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(Utils.Storage.GetAppDataFolderPath()))
                .ProtectKeysWithCertificate(new X509Certificate2(Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Path"), Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Password"), X509KeyStorageFlags.MachineKeySet));
            }

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
            });

            services.AddRazorPages()
                .AddNewtonsoftJson();

            var builder = new SqliteConnectionStringBuilder
            {
                DataSource = Path.Combine(Utils.Storage.GetAppDataFolderPath(), Constants.DatabaseFileName)
            };
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(builder.ConnectionString));

            services.AddSignalR();
            
            services.AddSingleton<Manager>();
            services.AddScoped<ViewDataHelper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,ApplicationDbContext context, Manager manager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCookiePolicy();

            app.UseSignalR(routes =>
            {
                routes.MapHub<BoincInfoHub>("/boincInfoHub");
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            // Initialize the Application
            BoincManager.Utils.InitializeApplication(context, manager, false);            

            // Start the Boinc Manager
            manager.Start();
        }
    }
}
