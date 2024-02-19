using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RuslanAPI.Core.Models;
using RuslanAPI.DataLayer.Data;
using RuslanAPI.Services.Authorization;
using RuslanAPI.Services.Mappers;
using RuslanAPI.Services.UserServices;
using System.Reflection;

namespace RuslanAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<UserDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DataBase")));
            builder.Services.AddTransient<IAuthService, AuthService>();
            builder.Services.AddTransient<IUserMapper, UserMapper>();
            builder.Services.AddScoped<IDbRepository, DbRepository>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IPasswordHasher<User>, PasswordHasher<User>>();
            builder.Services.AddHttpContextAccessor();


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Mokymai Book API",
                    Description = "An ASP.NET Core Web API for managing Books with Users",
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                opt.IncludeXmlComments(xmlPath);
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please ender valid JWT token",
                    Name = "AuthorizationController",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });
                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme{Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }}, new List<string>()
                    }
                });
            });
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(options =>
           {
               var secretKey = builder.Configuration.GetSection("Jwt:Key").Value;
               var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey));
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   ValidIssuer = builder.Configuration.GetSection("Jwt:Issuer").Value,
                   ValidAudience = builder.Configuration.GetSection("Jwt:Audience").Value,
                   IssuerSigningKey = key
               };
           });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            //builder.Services.AddCors(options =>
            //{
            //    options.AddPolicy(name: MyAllowSpecificOrigins,
            //                      policy =>
            //                      {
            //                          policy.AllowAnyOrigin()
            //                          .AllowAnyHeader()
            //                          .AllowAnyMethod();
            //                      });
            //});


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowSpecificOrigins");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}