using BusinessLayer.Interfaces;
using BusinessLayer.Services;
using CommonLayer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RepositoryLayer.Interfaces;
using RepositoryLayer.Services;
using System.Text;

namespace Fundoo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            var authenticationSettings = Configuration.GetSection("Settings");
            services.Configure<AuthenticationSettings>(authenticationSettings);

            //JWT authentication
            var authSettings = authenticationSettings.Get<AuthenticationSettings>();
            var key = Encoding.ASCII.GetBytes(authSettings.SecretKey);
            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwt =>
            {
                jwt.RequireHttpsMetadata = false;
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            //Database Connection string
            services.AddDbContext<UserContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:UserDB"]));
            services.AddScoped<IUserBL, UserBL>();
            services.AddScoped<IUserRL, UserDataBaseRL>();
            services.AddScoped<IEmailSender, EmailSender>();

            // Register the swagger generator, This service is responsible for genrating Swagger Documents.
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Fundoo API", Version = "v1" });
            });

            //Email Configurations here
            var emailConfigure = Configuration.GetSection("EmailSettings")
                .Get<EmailConfiguration>();
            services.AddSingleton(emailConfigure);

           


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fundoo API V1");

                // To serve SwaggerUI at application's root (http://localhost:<port>/) page, set the RoutePrefix property to an empty string
                c.RoutePrefix = string.Empty;

            });
        }
    }
}
