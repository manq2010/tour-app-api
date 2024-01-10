using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Touring.api.Data;
// using MySql.Data.MySqlClient;
// using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;
using Microsoft.Extensions.Logging;
// using Pomelo.EntityFrameworkCore.MySql.Storage;
using Microsoft.AspNetCore.Identity;
using Touring.api.Models;
using Touring.api.Data;
using Touring.api.Logic;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Touring.api.Data
{
    public class Startup
    {
        readonly string DefaultCorsPolicy = "DefaultCorsPolicy";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            var _connectionString = "";
            // _connectionString = Configuration.GetConnectionString("DatabaseConnection");
            _connectionString = Configuration.GetConnectionString("DefaultConnection");
            // var serverVersion = new ServerVersion(new Version(8, 0, 29));

            // Replace 'YourDbContext' with the name of your own DbContext derived class.
           // services.AddDbContext<RizeMzanziContext>(
           //     dbContextOptions => dbContextOptions
            //        .UseMySql(_connectionString));

            services.AddDbContextPool<AppDbContext>(options =>
            {
                options.UseSqlite(_connectionString);
            });

            services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            {
                // User defined password policy settings.  
                config.Password.RequiredLength = 4;
                config.Password.RequireDigit = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
            }).AddRoles<IdentityRole>()
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            // Adding Authentication  
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
           {
               options.SaveToken = true;
               options.RequireHttpsMetadata = false;
               options.TokenValidationParameters = new TokenValidationParameters()
               {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidAudience = Configuration["JWT:ValidAudience"],
                   ValidIssuer = Configuration["JWT:ValidIssuer"],
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
               };
           });


            //services.AddCors(options =>
            //{
            //    options.AddDefaultPolicy(builder =>
            //    {
            //        builder.AllowAnyOrigin()
            //               .AllowAnyHeader()
            //               .AllowAnyMethod();
            //    });

            //    options.AddPolicy(name: "DefaultCorsPolicy",
            //        builder =>
            //        {
            //            builder.AllowAnyOrigin()
            //                   .AllowAnyHeader()
            //                   .AllowAnyMethod();
            //        });
            //});

            services.AddCors(options =>
            {
                options.AddPolicy(DefaultCorsPolicy,
                builder =>
                {
                    builder.SetIsOriginAllowed(isOriginAllowed: _ => true).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                });
            });
            //services.ConfigureApplicationCookie(config =>
            //{
            //    config.Cookie.Name = "DemoProjectCookie";
            //    config.LoginPath = "/auth/Login"; // User defined login path  
            //    config.ExpireTimeSpan = TimeSpan.FromMinutes(5);
            //});

            //Add httpclient to call 3rd party APIs
            services.AddHttpClient();

            // services.AddSingleton<IUriService>(o =>
            // {
            //     var accessor = o.GetRequiredService<IHttpContextAccessor>();
            //     var request = accessor.HttpContext.Request;
            //     var uri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent() + request.PathBase);
            //     return new UriService(uri);
            // });

            //services.AddControllers();
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            // services.AddScoped<IRizemzanziRepo, SqlRizeMzanziRepo>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "rizemzanziAPI", Version = "v1" });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "rizemzanziAPI v1"));
            }

            //app.UseHttpsRedirection();//causes too many redirect

            app.UseRouting();
            //always enable cors AFTER routing 
            //app.UseCors(builder =>
            //{
            //    builder
            //    .AllowAnyOrigin()
            //    .AllowAnyMethod()
            //    .AllowAnyHeader();
            //});
            app.UseCors(DefaultCorsPolicy);


            app.UseAuthentication();//Add authentication pipeline
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
