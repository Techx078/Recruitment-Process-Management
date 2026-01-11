
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using System.Text.Json.Serialization;
using WebApis.Dtos;
using WebApis.Dtos.ForgotPasswordDtos;
using WebApis.Dtos.JobCandidateDtos;
using WebApis.Dtos.JobOpeningDto;
using WebApis.Repository;
using WebApis.Repository.CandidateRepository;
using WebApis.Repository.JobCandidateRepository;
using WebApis.Repository.JobOpeningRepository;
using WebApis.Repository.UserRepository;
using WebApis.Service;
using WebApis.Service.AuthorizationService;
using WebApis.Service.EmailService;
using WebApis.Service.ErrorHandlingService;
using WebApis.Service.ValidationService;
using WebApis.Service.ValidationService.AuthUserVallidator;
using WebApis.Service.ValidationService.CandidateValidator;
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
            builder.Services.AddScoped<ICandidateRepository, CandidateRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IJobOpeningRepository, JobOpeningRepository>();
            builder.Services.AddScoped<ICommonValidator<JobCandidateCreateDto>, JobCandidateCreateValidator>();
            builder.Services.AddScoped<ICommonValidator<ResetPasswordDto>,ResetPasswordValidator>();
            builder.Services.AddScoped<ICommonValidator<JobOpeningUpdateDto>, JobOpeningUpdateValidator>();
            // Add services to the container.
            builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    // This won't fix the bug, but might help you see the error details
                    options.SuppressInferBindingSourcesForParameters = true;
                })
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
            builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
            builder.Services.AddScoped<IAppEmailService, AppEmailService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ICommonValidator<JobOpeningDto>, JobOpeningValidator>();
            builder.Services.AddScoped<ICommonValidator<UpdateCandidateDto>,UpdateCandidateValidator>();
            builder.Services.AddScoped<ICommonValidator<RegisterCandidateDto>, RegisterCandidateValidator>();
            builder.Services.AddScoped<ICommonValidator<RegisterOtherUserDto>, RegisterOtherUserValidator>();
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
            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseAuthentication();
            
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
