using Library.Data;
using Library.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Library
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // register logging provider
            services.AddLogging(builder =>
            {
                builder.AddConsole();
            });

            // register sql lite database 
            services.AddDbContext<LibraryContext>(options =>
            {
                options.UseSqlite(Configuration["ConnectionStrings:LibraryDatabase"]);
            });

            // TODO: Configure CORS, AntiForgeryToken and XSS 

            // configure jwt auth
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var key = Configuration["Jwt:Key"];
                var signingKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key));
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = signingKey                 
                    
                };
                options.RequireHttpsMetadata = false;
                options.IncludeErrorDetails = true;
            });

            services.AddControllers();

            services.AddEndpointsApiExplorer();

            // enable swagger to inour Bearer token in the request header
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Library API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            });

            // configure automapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // register library service
            services.AddScoped<ILibraryService, LibraryService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library API V1");
                    c.DocExpansion(DocExpansion.None);
                });
            }

            app.UseHttpsRedirection();            

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();                
            });
        }
    }
}