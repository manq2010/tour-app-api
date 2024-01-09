// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Cors;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.WebUtilities;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Options;
// // using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.IdentityModel.Tokens;
// using System.Collections.Generic;
// using System.IdentityModel.Tokens;
// using System.Linq;
// using System.Security.Claims;
// using System.Text;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Authentication;
// using Microsoft.AspNetCore.Authentication.Cookies;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.Extensions.DependencyInjection;


// using Touring.api.Data;
// using Touring.api.Models;


// var builder = WebApplication.CreateBuilder(args);

// // Add services to the container.
// builder.Services.AddDbContext<AppDbContext>(options=>options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// // add/setup Identity  
// // builder.Services.AddIdentity<ApplicationUser, IdentityRole>(config =>
// //             {
// //                 // User defined password policy settings.  
// //                 config.Password.RequiredLength = 4;
// //                 config.Password.RequireDigit = false;
// //                 config.Password.RequireNonAlphanumeric = false;
// //                 config.Password.RequireUppercase = false;
// //             }).AddRoles<IdentityRole>()
// //                 .AddRoleManager<RoleManager<IdentityRole>>()
// //                 .AddEntityFrameworkStores<AppDbContext>()
// //                 .AddDefaultTokenProviders();

// builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
//     .AddEntityFrameworkStores<AppDbContext>()
//     .AddDefaultTokenProviders();

// builder.Services.AddAuthentication(options =>
//             {
//                 options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//                 options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//                 options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//             })
//             // Adding Jwt Bearer  
//             .AddJwtBearer(options =>
//             {
//                 options.SaveToken = true;
//                 options.RequireHttpsMetadata = false;
//                 options.TokenValidationParameters = new TokenValidationParameters()
//                 {
//                     ValidateIssuer = true,
//                     ValidateAudience = true,
//                     ValidAudience = builder.Configuration["JWT:ValidAudience"],
//                     ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
//                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
//                 };
//         });

// // builder.Services.AddAuthentication(options =>
// // {
// //     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
// //     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
// // }).AddJwtBearer(options =>
// // {
// //     options.TokenValidationParameters = new TokenValidationParameters
// //     {
// //         ValidateIssuer = true,
// //         ValidateAudience = true,
// //         ValidateLifetime = true,
// //         ValidateIssuerSigningKey = true,
// //         ValidIssuer = Configuration["Jwt:Issuer"],
// //         ValidAudience = Configuration["Jwt:Issuer"],
// //         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
// //     };
// // });

// builder.Services.AddControllers();

// // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// var app = builder.Build();

// app.MapControllers();

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// app.UseCors(options =>{
//     options.AllowAnyHeader();
//     options.AllowAnyMethod();
//     options.AllowAnyOrigin();
// });

// app.UseHttpsRedirection();

// // app.Run();