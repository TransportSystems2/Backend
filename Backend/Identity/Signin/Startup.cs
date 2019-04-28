using System;
using System.IO;
using System.Reflection;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using TransportSystems.Backend.Identity.Core.Business;
using TransportSystems.Backend.Identity.Core.Data.Domain;
using TransportSystems.Backend.Identity.Core.Database;
using TransportSystems.Backend.Identity.Core.Domain.Interfaces;
using TransportSystems.Backend.Identity.Core.Interfaces;
using TransportSystems.Backend.Identity.Signin.Data;
using TransportSystems.Backend.Identity.Signin.Services;
using TransportSystems.Backend.Identity.Signin.Validation;

namespace TransportSystems.Backend.Identity.Signin
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
            services.AddTransient<ISmsService, SmsService>();

            var connectionString = Configuration.GetConnectionString("db");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddDbContext<IdentityContext>(options =>
                options.UseNpgsql(connectionString, b => b.MigrationsAssembly(migrationsAssembly)));

            services.AddIdentity<User, UserRole>()
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseFailureEvents = true;
            })
                .AddExtensionGrantValidator<PhoneNumberTokenGrantValidator>()
                .AddDeveloperSigningCredential()
                .AddAspNetIdentity<User>()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseNpgsql(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseNpgsql(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 30;
                });

                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info { Title = "Signin", Version = "v1" });
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(xmlPath);
                });


            ConfigureCustomServices(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            DbUserInitializator.Initialize(app);
            DbIdentityInitializator.InitializeDatabase(app);
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseIdentityServer();

            app.UseMvc();
            app.UseMvcWithDefaultRoute();
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "identity/signin/docs/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/identity/signin/docs/v1/swagger.json", "Signin api V1");
                c.RoutePrefix = "identity/signin/docs";
            });
        }

        protected virtual void ConfigureCustomServices(IServiceCollection services)
        {
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IUserService, UserService>();

            services.AddTransient<IPushTokenRepository, PushTokenRepository>();
            services.AddTransient<IPushTokenService, PushTokenService>();

            services.AddTransient<IEventService, HandlerService>();
        }
    }
}
