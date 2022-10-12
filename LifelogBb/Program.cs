using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System.Text;
using AutoMapper;
using LifelogBb.Interfaces;
using LifelogBb.ApiServices;
using LifelogBb.ApiRepositories;
using LifelogBb.Models;
using Microsoft.OpenApi.Models;

namespace LifelogBb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;
            var config = builder.Configuration;

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
                    In = ParameterLocation.Header,
                    Description = "Please enter the JWT.",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            }); // Swagger

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Add all Repositories
            services.AddScoped(typeof(IRepository<>), typeof(EntityRepository<>));

            // Add scoped (per client connection) services
            services.AddScoped<WeightsService>();
            services.AddScoped<JournalsService>();
            services.AddScoped<StrengthTrainingsService>();
            services.AddScoped<EnduranceTrainingsService>();

            ConfigureCookieJwt(services, config);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
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
            .AddPolicyScheme("JWT_OR_COOKIE", "JWT_OR_COOKIE", options =>
            {
                options.ForwardDefaultSelector = context =>
                {
                    string authorization = context.Request.Headers[HeaderNames.Authorization];
                    if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
                        return JwtBearerDefaults.AuthenticationScheme;

                    return CookieAuthenticationDefaults.AuthenticationScheme;
                };
            });
        }
    }
}