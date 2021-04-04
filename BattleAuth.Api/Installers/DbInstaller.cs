namespace BattleAuth.Api.Installers
{
    using System;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
    using Service;

    public class DbInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["DefaultConnection"];

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseMySql(connectionString, new MySqlServerVersion(new Version(5, 7, 12)), m =>
                    {
                        m.MigrationsAssembly("BattleAuth.Api");
                        m.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null!);
                        m.CharSetBehavior(CharSetBehavior.AppendToAllColumns);
                        m.CharSet(CharSet.Latin1);
                    }).UseLoggerFactory(LoggerFactory.Create(b => b.AddConsole().AddFilter(level => level >= LogLevel.Information)))
                    .EnableSensitiveDataLogging().EnableDetailedErrors();
            });
        }
    }
}
