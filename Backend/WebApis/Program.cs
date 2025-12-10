
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using System.Text.Json.Serialization;
using WebApis.Dtos.JobCandidateDtos;
using WebApis.Dtos.JobOpeningDto;
using WebApis.Repository;
using WebApis.Repository.JobCandidateRepository;
using WebApis.Service;
using WebApis.Service.EmailService;
using WebApis.Service.ValidationService;
using WebApis.Service.ValidationService.JobCandidateValidator;
using WebApis.Service.ValidationService.JobOpeningValidator;

namespace WebApis
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped(typeof(ICommonRepository<>), typeof(CommonRepository<>));
            builder.Services.AddScoped<IJobCandidateRepository, JobCandidateRepository>();
            builder.Services.AddScoped<ICommonValidator<JobCandidateCreateDto>, JobCandidateCreateValidator>();
            // Add services to the container.
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddDbContext<Data.AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("AppDb"))
            );

            builder.Services.AddScoped<JwtService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<ICommonValidator<JobOpeningDto>, JobOpeningValidator>();
            

            builder.Services.AddCors(options => options.AddPolicy("MyLocalPolicy", policy =>
            {
                policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            }));

            builder.Services.AddSingleton<CloudinaryService>();

            var jwtSettings = builder.Configuration.GetSection("Jwt");
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
                    };
                });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();
            app.UseCors("MyLocalPolicy");
            app.UseAuthentication();
            
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
