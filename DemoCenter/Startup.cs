using System.Text;
using DemoCenter.Brokers.DateTimes;
using DemoCenter.Brokers.Loggings;
using DemoCenter.Brokers.Storages;
using DemoCenter.Services.Foundations.Groups;
using DemoCenter.Services.Foundations.GroupStudents;
using DemoCenter.Services.Foundations.Students;
using DemoCenter.Services.Foundations.Subjects;
using DemoCenter.Services.Foundations.Teachers;
using DemoCenter.Services.Foundations.Users;
using DemoCenter.Services.Orchestrations;
using DemoCenter.Services.Processings.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace DemoCenter
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
            services.AddDbContext<StorageBroker>();
            services.AddControllers();

            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc(
                    name: "v1",
                    info: new OpenApiInfo { Title = "DemoCenter", Version = "v1" });
            });

            RegisterBrokers(services);
            AddFoundationServices(services);
            AddProcessingServices(services);
            AddOrchestrationServices(services);
            RegisterJwtConfigurations(services, Configuration);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DemoCenter v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
                endpoints.MapControllers());
        }
        private static void RegisterBrokers(IServiceCollection services)
        {
            services.AddTransient<IStorageBroker, StorageBroker>();
            services.AddTransient<ILoggingBroker, LoggingBroker>();
            services.AddTransient<IDateTimeBroker, DateTimeBroker>();

        }

        private static void AddFoundationServices(IServiceCollection services)
        {
            services.AddTransient<IGroupService, GroupService>();
            services.AddTransient<IStudentService, StudentService>();
            services.AddTransient<ISubjectService, SubjectService>();
            services.AddTransient<ITeacherService, TeacherService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IGroupStudentService, GroupStudentService>();
        }
        private static void AddProcessingServices(IServiceCollection services) =>
           services.AddTransient<IUserProcessingService, UserProcessingService>();

        private static void AddOrchestrationServices(IServiceCollection services) =>
            services.AddTransient<IUserSecurityOrchestrationService, UserSecurityOrchestrationService>();

        private static void RegisterJwtConfigurations(IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    string key = configuration.GetSection("Jwt").GetValue<string>("Key");
                    byte[] convertKeyToBytes = Encoding.UTF8.GetBytes(key);

                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(convertKeyToBytes),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RequireExpirationTime = true,
                        ValidateLifetime = true
                    };
                });
        }
    }
}
