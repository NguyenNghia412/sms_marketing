using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thongbao.be.infrastructure.external.BackgroundJob
{
    public static class ConfigureBackgroundJob
    {
        public static void ConfigureHangfire(
            this IServiceCollection services,
            string connectionString
        )
        {
            services.AddHangfire(configuration =>
                configuration
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    //.UseSerilogLogProvider()
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSqlServerStorage(connectionString)
            );

            JobStorage.Current = new SqlServerStorage(
                connectionString
            );

            services.AddHangfireServer();
        }

        public static IApplicationBuilder UseHangfireDashboardWithAuth(this IApplicationBuilder app)
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                // Example: make dashboard read-only
                IsReadOnlyFunc = _ => true
            });

            return app;
        }
    }
}
