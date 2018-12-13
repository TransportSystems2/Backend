using AutoMapper;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using TransportSystems.Backend.Identity.Core.Business;
using TransportSystems.Backend.Identity.Core.Data.Domain;
using TransportSystems.Backend.Identity.Core.Database;
using TransportSystems.Backend.Identity.Core.Domain.Interfaces;
using TransportSystems.Backend.Identity.Core.Interfaces;

namespace TransportSystems.Backend.Identity.Manage
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddAutoMapper();

            ConfigureAuthentication(services);
            ConfigureApplicationContext(services);
            ConfigureIdentity(services);
            ConfigureCustomServices(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
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
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc();
        }

        protected virtual void ConfigureAuthentication(IServiceCollection services)
        {
            services.AddAuthorization();
            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
            {
                options.Authority = Configuration.GetConnectionString("identity");
                options.RequireHttpsMetadata = false;

                options.ApiName = "TSAPI";
            });
        }

        protected virtual void ConfigureApplicationContext(IServiceCollection services)
        {
            services.AddDbContext<IdentityContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("db"), b => b.MigrationsAssembly("TransportSystems.Backend.Identity.Manage")));
        }

        protected virtual void ConfigureIdentity(IServiceCollection services)
        {
            services.AddIdentity<User, UserRole>()
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders();
        }

        protected virtual void ConfigureCustomServices(IServiceCollection services)
        {
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IUserService, UserService>();

            services.AddTransient<IPushTokenRepository, PushTokenRepository>();
            services.AddTransient<IPushTokenService, PushTokenService>();
        }
    }
}
