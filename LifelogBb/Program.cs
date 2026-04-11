using LifelogBb.ApiRepositories;
using LifelogBb.ApiServices;
using LifelogBb.DTOs;
using LifelogBb.Interfaces;
using LifelogBb.Models;
using LifelogBb.Utilities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Text;

namespace LifelogBb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;
            var config = builder.Configuration;

            // Logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Trace); // LogLevel.Information, LogLevel.Trace

            // Add services to the container.
            services.AddEntityFrameworkSqlite().AddDbContext<LifelogBbContext>();

            services.AddControllersWithViews() // MVC controllers
                .AddMvcOptions(options => options.Filters.Add(new AuthorizeFilter())); // Add Authorize Attribute globally
            services.AddControllers() // API controllers
                .AddMvcOptions(options => options.Filters.Add(new AuthorizeFilter())); // Add Authorize Attribute globally
            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "LifelogBb API", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme."
                });
                // Swashbuckle v10+ / Microsoft.OpenApi v2: pass document to OpenApiSecuritySchemeReference so it resolves against the defined scheme
                option.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });
            }); // Swagger

            // MCP Server
            services.AddMcpServer()
                .AddAuthorizationFilters()
                .WithHttpTransport(options => { options.Stateless = true; })
                .WithToolsFromAssembly();

            // https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-7.0
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            // AutoMapper v16+: scan the entire assembly for all Profile subclasses
            services.AddAutoMapper(cfg => { }, typeof(Program));

            // Add all Repositories
            services.AddScoped(typeof(IRepository<>), typeof(EntityRepository<>));

            // Add scoped (per client connection) services
            services.AddScoped<WeightsService>();
            services.AddScoped<JournalsService>();
            services.AddScoped<StrengthTrainingsService>();
            services.AddScoped<EnduranceTrainingsService>();
            services.AddScoped<QuotesService>();
            services.AddScoped<TodosService>();
            services.AddScoped<HabitsService>();
            services.AddScoped<GoalsService>();

            ConfigureCookieJwt(services, config);

            var app = builder.Build();

            app.UseForwardedHeaders();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                // app.UseHsts();
            }

            // Seed demo data in development
            if (app.Environment.IsDevelopment())
            {
                using (var scope = app.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<LifelogBbContext>();
                    DbSeeder.Seed(db);
                }
            }

            // app.UseHttpsRedirection(); // Let nginx reverse proxy handle this
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapControllers();

            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI(options => {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.EnablePersistAuthorization();
            });

            // MCP
            app.MapMcp("mcp").RequireAuthorization();

            app.Run();
        }

        private static void ConfigureCookieJwt(IServiceCollection services, ConfigurationManager config)
        {
            // Configure Cookie and JWT Auth => https://weblog.west-wind.com/posts/2022/Mar/29/Combining-Bearer-Token-and-Cookie-Auth-in-ASPNET
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "JWT_OR_COOKIE";
                options.DefaultChallengeScheme = "JWT_OR_COOKIE";
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login/";
                options.ExpireTimeSpan = TimeSpan.FromDays(double.Parse(config["Authentication:Cookie:ExpireDays"]));
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = config["Authentication:JwtToken:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = config["Authentication:JwtToken:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Authentication:JwtToken:SigningKey"]))
                };
            })
            .AddMcp()
            .AddPolicyScheme("JWT_OR_COOKIE", "JWT_OR_COOKIE", options =>
            {
                options.ForwardDefaultSelector = context =>
                {
                    string authorization = context.Request.Headers[HeaderNames.Authorization];
                    if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
                        return JwtBearerDefaults.AuthenticationScheme;
                    else if (!string.IsNullOrEmpty(authorization))
                        return JwtBearerDefaults.AuthenticationScheme;

                    return CookieAuthenticationDefaults.AuthenticationScheme;
                };
            });
        }
    }
}
