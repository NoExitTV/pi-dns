using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pi.Dns.Common.Settings;
using Pi.Dns.Common.Stores;
using Pi.Dns.Common.Stores.Storage;
using Pi.Dns.Web.Statistics.Settings;
using Serilog;

namespace Pi.Dns.Web.Statistics
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            // Settings
            services.Configure<DnsServerStatisticsStoreSettings>(Configuration.GetSection(DnsServerStatisticsStoreSettings.ConfigSectionName));
            services.Configure<DisplayableDnsServerSettings>(Configuration.GetSection(DisplayableDnsServerSettings.ConfigSectionName));

            // Stores
            services.AddSingleton<IDnsServerStatisticsStore, DnsServerStatisticsStorage>();

            services.AddMemoryCache();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Proxy support (nginx)
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
