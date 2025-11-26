using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using API.DataAccess.DataAccess.DB;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // Prevent "possible object cycle" System.Text.Json exception by ignoring cyclic references.
                    // Alternatively use ReferenceHandler.Preserve if you need $id/$ref-based payloads.
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.MaxDepth = 64;
                });

            // configure CORS
            var corsPolicy = "CorsPolicy";
            var corsSection = builder.Configuration.GetSection("Cors");
            var allowedOrigins = corsSection.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(corsPolicy, policy =>
                {
                    if (allowedOrigins.Length == 0 || (allowedOrigins.Length == 1 && allowedOrigins[0] == "*"))
                    {
                        policy.AllowAnyOrigin();
                    }
                    else
                    {
                        policy.WithOrigins(allowedOrigins)
                              .SetIsOriginAllowedToAllowWildcardSubdomains();
                    }
                    policy.AllowAnyOrigin();
                    policy.AllowAnyMethod();
                    policy.AllowAnyHeader();
                });
            });
            
            // Configure EF Core
            var useInMemory = builder.Configuration.GetValue<bool>("UseInMemory");
            if (useInMemory)
            {
                // optional in-memory DB
                builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("FociDB"));
            }
            else
            {
                builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            }

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

                // Include XML comments (enable XML docs in the project file or via Project Properties)
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });

            var app = builder.Build();

            // enable virtual directory deployment
            var pathBase = builder.Configuration["PathBase"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
                app.Use((context, next) =>
                {
                    if (context.Request.Path.StartsWithSegments(pathBase))
                        return next(context);
                    context.Response.StatusCode = 404;
                    return Task.CompletedTask;
                });
            }

            app.UseSwagger();
            app.UseSwaggerUI();

            // don't need HTTPS for this demo app
            // app.UseHttpsRedirection();

            app.UseCors(corsPolicy);

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
